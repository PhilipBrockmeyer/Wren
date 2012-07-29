using System.Linq;
using Wren.Core;
using Wren.Core.GameLibrary;
using System;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Interop;
using StructureMap;
using System.Windows.Controls;
using Wren.Core.Events;
using Wren.EventHandlers;
using System.Collections.ObjectModel;
using Wren.Module.Core.UI.Windows;
using System.Windows.Data;
using System.Threading;
using Wren.Core.Commands;
using Wren.Core.Settings;
using Wren.Core.Replay;
using Wren.Core.Input;
using Wren.Core.Statistics;
using Wren.Core.Debugging;
using Wren.Core.GameEvents;
using Wren.Emulation.MasterSystem;
using Wren.Views;
using Wren.ViewModels;
using Wren.Core.Directory;
using System.Diagnostics;
using Wren.Core.Persistence;
using Wren.Core.Audio;

namespace Wren
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        EmulatorView _ev;
        ServiceLocator _serviceLocator;
        IEmulationRunner _runner;
        GameInfo _runningGame;
        GameInfo _previouslySelectedGame;
        Boolean _isInGameMode = false;
        IInputSource _uiInputSource;
        IInputSource _gameInputSource;
        Guid _fullScreenEmulationRunner;
        DispatcherTimer _gameChangeTimer;
        DispatcherTimer _inputTimer;
        ListCollectionView _collectionView;
        SortingOptions _sortingOption;

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public Window1()
        {
            InitializeComponent();
          
            _runner = null;
            _ev = new EmulatorView();
            _ev.VerticalAlignment = VerticalAlignment.Stretch;
            _ev.HorizontalAlignment = HorizontalAlignment.Stretch;
            main.Content = _ev;
            var helperElement = new FrameworkElement();
            helperElement.Loaded += new RoutedEventHandler(_ev_Loaded);
            helper.Content = helperElement;
            
            RavenDbPersistenceProvider runRavenDbStaticConstructor = new RavenDbPersistenceProvider();

            _gameChangeTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);
            _gameChangeTimer.Interval = TimeSpan.FromMilliseconds(500);
            _gameChangeTimer.Tick += new EventHandler(timer_Tick);
            
            _inputTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            _inputTimer.Interval = TimeSpan.FromMilliseconds(50);
            _inputTimer.Tick += new EventHandler(inputTimer_Tick);
            
            Boolean fullScreen = false;

            if (fullScreen)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                Topmost = true;
                WindowState = WindowState.Maximized;
            }

            WrenWindows.MemoryFilterViewModel = new MemoryFilterViewModel();
            WrenWindows.MemoryFilterWindow = new MemoryFilter(WrenWindows.MemoryFilterViewModel);

            WrenCore.Initialize();

            _serviceLocator = WrenCore.GetServiceLocator();

            var settingsManager = _serviceLocator.GetInstance<ISettingsManager>();
            settingsManager.RegisterSettings<LogOnSettings>();
            settingsManager.Load();

            _serviceLocator.RegisterSingleton<IEmulatorSurfaces, EmulatorSurfaces>();
            ObjectFactory.Configure(c =>
            {
                c.ForRequestedType<Image>().TheDefault.Is.ConstructedBy(() => this._ev.renderSurface);
                c.ForRequestedType<Dispatcher>().TheDefault.Is.ConstructedBy(() => this.Dispatcher);
            });

            var eventAggregator = _serviceLocator.GetInstance<IEventAggregator>();

            eventAggregator.Subscribe<LoggedOnEvent>(LogOn);
            eventAggregator.Subscribe<SettingsAppliedEvent>(SettingsApplied);
            popupPresenter.Content = new LogOnView(new LogOnViewModel(eventAggregator, settingsManager));

            SmsEmulator sms = new SmsEmulator();
            sms.Initialize(eventAggregator);

            RegisterEventHandlers(eventAggregator);
        }

        void inputTimer_Tick(object sender, EventArgs e)
        {
            if (_isInGameMode)
            {
                var inputState = _gameInputSource.GetCurrentInputState();

                if (inputState.GetIsButtonPresed(100))
                {
                    _runner.SendCommand(new QuitCommand());
                }

                if (inputState.GetIsButtonPresed(101))
                {
                    _runner.SendCommand(new MemoryDumpCommand());
                }
             }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (_isInGameMode)
                return;

            if (_runningGame == gamesList.SelectedItem)
                return;

            if (_previouslySelectedGame != gamesList.SelectedItem)
            {
                _previouslySelectedGame = gamesList.SelectedItem as GameInfo;
                return;
            }

            _runningGame = gamesList.SelectedItem as GameInfo;

            if (_runningGame != null)
                _serviceLocator.GetInstance<IEventAggregator>().Publish(new GameSelectedEvent(_runningGame));

            PreviewEmulator();
        }

        void _ev_Loaded(object sender, RoutedEventArgs e)
        {
            WrenWindows.Dispatcher = this.Dispatcher;
            WrenCore.WindowHandle = new WindowInteropHelper(this).Handle;
            
            InitializeInput();

            this.Visibility = System.Windows.Visibility.Visible;
        }

        private void InitializeInput()
        {
            var inputPipeline = _serviceLocator.GetInstance<IInputSourceAssembler>();
            _uiInputSource = inputPipeline.BuildInputSource(new EmulationContext(Game.Empty, new EmulatedSystem("WrenUI")));
            _gameInputSource = inputPipeline.BuildInputSource(new EmulationContext(Game.Empty, new EmulatedSystem("WrenGame")));
        }

        private void LoadGameLibrary()
        {
            var library = _serviceLocator.GetInstance<IGameLibraryManager>();
            var gameInfoList = new ObservableCollection<GameInfo>();
            var collection = new ThreadedObservableCollection<GameInfo>(gameInfoList, this.Dispatcher);
            _collectionView = new ListCollectionView(gameInfoList);
            _collectionView.Filter = new Predicate<object>(Filter);
            _collectionView.CustomSort = new GameInfoComparer(SortingOptions.Name);
            _collectionView.GroupDescriptions.Add(new GameInfoGroupDescription(SortingOptions.Name));
            _sortingOption = SortingOptions.Name;
            Thread gameLoader = new Thread(() => library.UpdateGameLibrary(collection));
            gameLoader.Start();
            gamesList.ItemsSource = _collectionView;
            gamesList.SelectedIndex = 0;
        }

        private void RegisterEventHandlers(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe<MemoryDumpedEvent, MemoryDumpedEventHandler>();
            eventAggregator.Subscribe<RenderingSurfaceCreatedEvent, RenderingSurfaceCreatedEventHandler>();
            eventAggregator.Subscribe<FrameRenderedEvent, FrameRenderedEventHandler>();
            eventAggregator.Subscribe<EmulatorQuitEvent>(QuitGame);
        }

        protected void QuitGame(IEvent e)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    var ev = e as EmulatorQuitEvent;
                    if (ev.EmulationRunnerId == _fullScreenEmulationRunner)
                    {
                        this.IsEnabled = true;

                        fullscreenCanvas.Children.Remove(_ev);
                        main.Content = _ev;

                        _isInGameMode = false;
                        _runningGame = null;
                    }
                }));
        }

        protected Boolean Filter(object o)
        {
            return !String.IsNullOrEmpty((String)((GameInfo)o).GetValue("Year"));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_runner != null)
                _runner.SendCommand(new QuitCommand());

            _gameChangeTimer.Stop();
            _inputTimer.Stop();

            var frtf = _serviceLocator.GetInstance<IFrameRateTimerFactory>();
            frtf.ShutDown();

            if (_serviceLocator != null)
                _serviceLocator.GetInstance<IStatisticsManager>().SaveStatistics();

            WrenCore.Unload();            
            
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Process.GetCurrentProcess().Kill();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void RunEmulator()
        {
            GameInfo gi = gamesList.SelectedItem as GameInfo;

            if (_runner != null)
                _runner.SendCommand(new QuitCommand());

            _runner = _serviceLocator.GetInstance<IEmulationRunner>();

            var eContext = new EmulationContext(gi.Game, new EmulatedSystem((String)gi.GetValue("System")));
            var eventAggregator = _serviceLocator.GetInstance<IEventAggregator>();

            if (_isInGameMode)
            {
                /*var statisticsManager = _serviceLocator.GetInstance<IStatisticsManager>();
                statisticsManager.UpdateStatisticDefinitions(eContext.Game);
                statisticsManager.InitializeStatistics(eContext.Game, new GameEventAggregator(eventAggregator, _runner));*/
            }
            
            _runner.Start(eContext, eventAggregator, _isInGameMode ? EmulationMode.Playing : EmulationMode.Preview);
            _fullScreenEmulationRunner = _runner.InstanceId;
        }

        private void PreviewEmulator()
        {
            var settingsManager = _serviceLocator.GetInstance<ISettingsManager>();
            var replayManager = _serviceLocator.GetInstance<IReplayManager>();
            GameInfo gi = gamesList.SelectedItem as GameInfo;

            if (gi == null)
                return;

            var replay = replayManager.GetGameReplays(gi.Game).FirstOrDefault();

            ReplaySettings rs = new ReplaySettings();
            rs.IsRecording = false;

            if (replay == null)
            {
                rs.IsPlayingBack = false;
            }
            else
            {
                rs.IsPlayingBack = true;
                rs.FileName = replay.FileName;
            }

            InputSettings ins = new InputSettings()
            {
                IsUserInputEnabled = false
            };

            settingsManager.ApplySettings(rs, true);
            settingsManager.ApplySettings(ins, true);

            RunEmulator();
        }

        private void LogOn(IEvent e)
        {
            _inputTimer.Stop();
            _gameChangeTimer.Stop();

            popupPresenter.Content = null;

            LoggedOnEvent ev = e as LoggedOnEvent;
            WrenCore.UserId = ev.UserId;

            // Open rom path setting window if no paths are set.
            var dManager = _serviceLocator.GetInstance<IDirectoryManager>();
            if (dManager.GetPaths(EmulationContext.Empty, GameLibraryModule.RomPathKey).Count() == 0)
            {
                popupPresenter.Content = ObjectFactory.GetInstance<SettingsView>();
            }

            LoadGameLibrary();
            _inputTimer.Start();
            _gameChangeTimer.Start();
        }

        private void SettingsApplied(IEvent e)
        {
            LoadGameLibrary();

            popupPresenter.Content = null;

            _inputTimer.Start();
            _gameChangeTimer.Start();
        }

        private void PlayEmulator()
        {
            var gi = gamesList.SelectedItem as GameInfo;

            if (gi == null)
                return;

            // this.IsEnabled = false;

            main.Content = null;
            fullscreenCanvas.Children.Add(_ev);

            _isInGameMode = true;

            String fileName = Guid.NewGuid().ToString() + ".wr";

            var settingsManager = _serviceLocator.GetInstance<ISettingsManager>();
            var replayManager = _serviceLocator.GetInstance<IReplayManager>();
            
            // replayManager.AddReplay(new RecordedReplay(gi.Game.Id, fileName, DateTime.Now));

            ReplaySettings rs = new ReplaySettings()
            {
                IsRecording = false,
                IsPlayingBack = false,
                FileName = fileName
            };

            InputSettings ins = new InputSettings()
            {
                IsUserInputEnabled = true
            };

            settingsManager.ApplySettings(rs, true);
            settingsManager.ApplySettings(ins, true);

            RunEmulator();
        }

        private void ChangeSorting_Click(object sender, RoutedEventArgs e)
        {
            _collectionView.GroupDescriptions.Clear();

            if (_sortingOption == SortingOptions.Name)
            {
                _sortingOption = SortingOptions.Publisher;
                _collectionView.CustomSort = new GameInfoComparer(_sortingOption);
                _collectionView.GroupDescriptions.Add(new GameInfoGroupDescription(_sortingOption));
                return;
            }

            if (_sortingOption == SortingOptions.Publisher)
            {
                _sortingOption = SortingOptions.Year;
                _collectionView.CustomSort = new GameInfoComparer(_sortingOption);
                _collectionView.GroupDescriptions.Add(new GameInfoGroupDescription(_sortingOption));
                return;
            }
            
            if (_sortingOption == SortingOptions.Year)
            {
                _sortingOption = SortingOptions.Name;
                _collectionView.CustomSort = new GameInfoComparer(_sortingOption);
                _collectionView.GroupDescriptions.Add(new GameInfoGroupDescription(_sortingOption));
                return;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlayEmulator();
            //_runner.SendCommand(new MemoryDumpCommand());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _inputTimer.Stop();
            _gameChangeTimer.Stop();
            popupPresenter.Content = ObjectFactory.GetInstance<SettingsView>();
        }
    }
}

