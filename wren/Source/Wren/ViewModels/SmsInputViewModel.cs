using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;
using Wren.Core.Input;
using Wren.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Wren.ViewModels
{
    public class SmsInputViewModel : INotifyPropertyChanged
    {
        ISettingsManager _settingsManager;
        KeyboardBindingSettings _keyboardSettings;
        GamepadBindingSettings _gamepadSettings;

        IList<KeyboardBindingSettings.KeyBinding> _newKeyboardBindings;
        IList<GamepadBindingSettings.GamepadBinding> _newGamepad1BindingSettings;
        IList<GamepadBindingSettings.GamepadBinding> _newGamepad2BindingSettings;

        private String _PlayerOneUp;

        public String PlayerOneUp
        {
            get { return _PlayerOneUp; }
            set
            {
                _PlayerOneUp = value;
                OnPropertyChanged("PlayerOneUp");
            }
        }

        private String _PlayerOneDown;

        public String PlayerOneDown
        {
            get { return _PlayerOneDown; }
            set
            {
                _PlayerOneDown = value;
                OnPropertyChanged("PlayerOneDown");
            }
        }

        private String _PlayerOneLeft;

        public String PlayerOneLeft
        {
            get { return _PlayerOneLeft; }
            set
            {
                _PlayerOneLeft = value;
                OnPropertyChanged("PlayerOneLeft");
            }
        }


        private String _PlayerOneRight;

        public String PlayerOneRight
        {
            get { return _PlayerOneRight; }
            set
            {
                _PlayerOneRight = value;
                OnPropertyChanged("PlayerOneRight");
            }
        }

        private String _PlayerOneA;

        public String PlayerOneA
        {
            get { return _PlayerOneA; }
            set
            {
                _PlayerOneA = value;
                OnPropertyChanged("PlayerOneA");
            }
        }


        private String _PlayerOneB;

        public String PlayerOneB
        {
            get { return _PlayerOneB; }
            set
            {
                _PlayerOneB = value;
                OnPropertyChanged("PlayerOneB");
            }
        }

        private String _PlayerTwoUp;

        public String PlayerTwoUp
        {
            get { return _PlayerTwoUp; }
            set
            {
                _PlayerTwoUp = value;
                OnPropertyChanged("PlayerTwoUp");
            }
        }


        private String _PlayerTwoDown;

        public String PlayerTwoDown
        {
            get { return _PlayerTwoDown; }
            set
            {
                _PlayerTwoDown = value;
                OnPropertyChanged("PlayerTwoDown");
            }
        }

        private String _PlayerTwoLeft;

        public String PlayerTwoLeft
        {
            get { return _PlayerTwoLeft; }
            set
            {
                _PlayerTwoLeft = value;
                OnPropertyChanged("PlayerTwoLeft");
            }
        }

        private String _PlayerTwoRight;

        public String PlayerTwoRight
        {
            get { return _PlayerTwoRight; }
            set
            {
                _PlayerTwoRight = value;
                OnPropertyChanged("PlayerTwoRight");
            }
        }

        private String _PlayerTwoA;

        public String PlayerTwoA
        {
            get { return _PlayerTwoA; }
            set
            {
                _PlayerTwoA = value;
                OnPropertyChanged("PlayerTwoA");
            }
        }

        private String _PlayerTwoB;

        public String PlayerTwoB
        {
            get { return _PlayerTwoB; }
            set
            {
                _PlayerTwoB = value;
                OnPropertyChanged("PlayerTwoB");
            }
        }

        List<KeyboardBindingSettings.KeyBinding> KeyboardBindings { get; set; }

        public SmsInputViewModel(ISettingsManager settingsManager, InputManager inputManager)
        {
            _settingsManager = settingsManager;
            _keyboardSettings = _settingsManager.LoadSettings<KeyboardBindingSettings>(new EmulationContext(new Game(String.Empty, String.Empty), new EmulatedSystem("SMS")));
            _gamepadSettings = _settingsManager.LoadSettings<GamepadBindingSettings>(new EmulationContext(new Game(String.Empty, String.Empty), new EmulatedSystem("SMS")));
            _newKeyboardBindings = new List<KeyboardBindingSettings.KeyBinding>(_keyboardSettings.Bindings);
            _newGamepad1BindingSettings = new List<GamepadBindingSettings.GamepadBinding>(_gamepadSettings.Gamepad1Bindings);
            _newGamepad2BindingSettings = new List<GamepadBindingSettings.GamepadBinding>(_gamepadSettings.Gamepad2Bindings);

            UpdateStrings();
        }

        private void UpdateStrings()
        {
            PlayerOneUp = BindingsToString(0);
            PlayerOneDown = BindingsToString(1);
            PlayerOneLeft = BindingsToString(2);
            PlayerOneRight = BindingsToString(3);
            PlayerOneA = BindingsToString(4);
            PlayerOneB = BindingsToString(5);

            PlayerTwoUp = BindingsToString(8);
            PlayerTwoDown = BindingsToString(9);
            PlayerTwoLeft = BindingsToString(10);
            PlayerTwoRight = BindingsToString(11);
            PlayerTwoA = BindingsToString(12);
            PlayerTwoB = BindingsToString(13);
        }

        private String BindingsToString(Int32 buttonId)
        {
            StringBuilder output = new StringBuilder();

            foreach (var kbinding in _newKeyboardBindings.Where(binding => binding.ButtonId == buttonId))
            {
                output.Append(kbinding.Key)
                      .Append(" OR ");
            }

            foreach (var gpbinding in _newGamepad1BindingSettings.Where(binding => binding.ButtonId == buttonId))
            {
                output.Append(String.Format(gpbinding.GetButtonName(), 1))
                      .Append(" OR ");
            }

            foreach (var gpbinding in _newGamepad2BindingSettings.Where(binding => binding.ButtonId == buttonId))
            {
                output.Append(String.Format(gpbinding.GetButtonName(), 2))
                      .Append(" OR ");
            }

            if (output.ToString().EndsWith(" OR "))
                output.Remove(output.Length - " OR ".Length, " OR ".Length);
            
            return output.ToString();
        }

        public void UpdateBindingsFromStrings()
        {
            _newGamepad1BindingSettings.Clear();
            _newGamepad2BindingSettings.Clear();
            _newKeyboardBindings.Clear();

            StringToBindings(PlayerOneUp, 0);
            StringToBindings(PlayerOneDown, 1);
            StringToBindings(PlayerOneLeft, 2);
            StringToBindings(PlayerOneRight, 3);
            StringToBindings(PlayerOneA, 4);
            StringToBindings(PlayerOneB, 5);

            StringToBindings(PlayerTwoUp, 8);
            StringToBindings(PlayerTwoDown, 9);
            StringToBindings(PlayerTwoLeft, 10);
            StringToBindings(PlayerTwoRight, 11);
            StringToBindings(PlayerTwoA, 12);
            StringToBindings(PlayerTwoB, 13);
        }

        private void StringToBindings(String bindingString, Int32 buttonId)
        {
            foreach (var bindingPart in bindingString.Split(new String[] {" OR "}, 10, StringSplitOptions.None))
            {
                if (String.IsNullOrWhiteSpace(bindingPart))
                    continue;

                if (bindingPart.StartsWith("Gamepad"))
                {
                    Int32 gamepadIndex = Int32.Parse(bindingPart.Substring("Gamepad ".Length, 1));
                    Int32 button = GamepadBindingSettings.GamepadBinding.ButtonFromString(bindingPart.Substring("Gamepad ".Length + 2));

                    if (gamepadIndex == 1)
                        _newGamepad1BindingSettings.Add(new GamepadBindingSettings.GamepadBinding() { ButtonId = buttonId, Button = button });

                    if (gamepadIndex == 2)
                        _newGamepad2BindingSettings.Add(new GamepadBindingSettings.GamepadBinding() { ButtonId = buttonId, Button = button });
                }
                else
                {
                    _newKeyboardBindings.Add(new KeyboardBindingSettings.KeyBinding() { Key = bindingPart, ButtonId = buttonId });
                }
            }
        }

        public void Save()
        {
            UpdateBindingsFromStrings();

            var keyboardSettings = new KeyboardBindingSettings() { Bindings = _newKeyboardBindings.ToArray() };
            var gamepadSettings = new GamepadBindingSettings() { Gamepad1Bindings = _newGamepad1BindingSettings.ToArray(), Gamepad2Bindings = _newGamepad2BindingSettings.ToArray() };

            var context = new EmulationContext(new Game(String.Empty, String.Empty), new EmulatedSystem("SMS"));
            _settingsManager.ApplySettings(keyboardSettings, context, SettingsScope.EmulatedSystem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddKeyBinding(System.Windows.Input.Key key, Int32 buttonId)
        {
            String keyName = key.ToString();

            if (key == System.Windows.Input.Key.Up)
                keyName = "UpArrow";
            if (key == System.Windows.Input.Key.Down)
                keyName = "DownArrow"; 
            if (key == System.Windows.Input.Key.Left)
                keyName = "LeftArrow"; 
            if (key == System.Windows.Input.Key.Right)
                keyName = "RightArrow";

            if (_newKeyboardBindings.Where(b => b.ButtonId == buttonId && b.Key == keyName).Count() > 0)
                return;

            _newKeyboardBindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = buttonId, Key = keyName });
            UpdateStrings();
        }

        public void AddGamepadBinding(Int32 button, Int32 gamepadIndex, Int32 buttonId)
        {
            if (buttonId == -1)
                return;

            if (gamepadIndex == 1)
            {
                if (_newGamepad1BindingSettings.Where(b => b.ButtonId == buttonId && b.Button == button).Count() > 0)
                    return;

                _newGamepad1BindingSettings.Add(new GamepadBindingSettings.GamepadBinding() { Button = button, ButtonId = buttonId });
            }
            else
            {
                if (_newGamepad2BindingSettings.Where(b => b.ButtonId == buttonId && b.Button == button).Count() > 0)
                    return;

                _newGamepad2BindingSettings.Add(new GamepadBindingSettings.GamepadBinding() { Button = button, ButtonId = buttonId });
            }

            UpdateStrings();
        }
    }
}
