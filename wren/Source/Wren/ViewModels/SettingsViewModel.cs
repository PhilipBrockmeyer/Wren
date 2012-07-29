using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;
using Wren.Core;
using Wren.Core.Events;
using Wren.Core.Input;

namespace Wren.ViewModels
{
    public class SettingsViewModel
    {
        ISettingsManager _settingsManager;
        IEventAggregator _eventAggregator;

        public RomPathsViewModel RomPathsViewModel { get; set; }
        public SmsInputViewModel SmsInputViewModel { get; set; }
        public InputManager InputManager { get; set; }

        public SettingsViewModel(ISettingsManager settingsManager, 
                                 IEventAggregator eventAggregator, 
                                 RomPathsViewModel romPathsViewModel,
                                 SmsInputViewModel smsInputViewModel, 
                                 InputManager inputManager )
        {
            _settingsManager = settingsManager;
            _eventAggregator = eventAggregator;
            RomPathsViewModel = romPathsViewModel;
            SmsInputViewModel = smsInputViewModel;
            InputManager = inputManager;
        }

        public void Save()
        {
            RomPathsViewModel.Save();
            SmsInputViewModel.Save();
            _settingsManager.SaveSettings();

            _eventAggregator.Publish(new SettingsAppliedEvent());
        }

        public void Cancel()
        {
            _eventAggregator.Publish(new SettingsAppliedEvent());
        }
    }
}
