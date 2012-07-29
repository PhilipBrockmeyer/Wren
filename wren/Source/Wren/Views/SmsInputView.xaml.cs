using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wren.ViewModels;
using System.Windows.Threading;
using SlimDX.DirectInput;

namespace Wren.Views
{
    /// <summary>
    /// Interaction logic for SmsInputView.xaml
    /// </summary>
    public partial class SmsInputView : UserControl
    {
        SmsInputViewModel _viewModel;
        Wren.Core.Input.InputManager _inputManager;
        Joystick _gamepad1;
        Joystick _gamepad2;
        DispatcherTimer _deviceListener;

        public SmsInputView(SmsInputViewModel viewModel, Wren.Core.Input.InputManager inputManager)
        {
            InitializeComponent();

            _viewModel = viewModel;
            this.DataContext = _viewModel;

            _inputManager = inputManager;
            _deviceListener = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);


            _deviceListener.Tick += new EventHandler(_deviceListener_Tick);
            _deviceListener.Interval = TimeSpan.FromMilliseconds(300);
        }

        public void StartListening()
        {
            _deviceListener.Start();

            var gamepadCount = _inputManager.GetJoystickCount();

            if (gamepadCount > 0)
                _gamepad1 = _inputManager.GetJoystick(0);

            if (gamepadCount > 1)
                _gamepad2 = _inputManager.GetJoystick(1);


            if (_gamepad1 != null)
            {
                _gamepad1.Acquire();
            }

            if (_gamepad2 != null)
            {
                _gamepad2.Acquire();
            }

        }

        public void StopListening()
        {
            _deviceListener.Stop();

            var gamepadCount = _inputManager.GetJoystickCount();

            if (gamepadCount > 0)
                _gamepad1 = _inputManager.GetJoystick(0);

            if (gamepadCount > 1)
                _gamepad2 = _inputManager.GetJoystick(1);


            if (_gamepad1 != null)
            {
                _gamepad1.Unacquire();
            }

            if (_gamepad2 != null)
            {
                _gamepad2.Unacquire();
            }
        }

        void _deviceListener_Tick(object sender, EventArgs e)
        {
            if (_gamepad1 != null)
            {
                _gamepad1.Poll();
                var state = _gamepad1.GetCurrentState();
                CheckButtonPresses(state, 1);
            }

            if (_gamepad2 != null)
            {
                _gamepad2.Poll();
                var state = _gamepad1.GetCurrentState();
                CheckButtonPresses(state, 2);
            }
        }

        private void CheckButtonPresses(JoystickState state, Int32 gamepadIndex)
        {
            if (state.Y < 0x4000)
                _viewModel.AddGamepadBinding(-1, gamepadIndex, GetFocusedButton());

            if (state.Y > 0xC000)
                _viewModel.AddGamepadBinding(-2, gamepadIndex, GetFocusedButton());

            if (state.X < 0x4000)
                _viewModel.AddGamepadBinding(-3, gamepadIndex, GetFocusedButton());

            if (state.X > 0xC000)
                _viewModel.AddGamepadBinding(-4, gamepadIndex, GetFocusedButton());

            Int32 buttonIndex = 0;

            for (buttonIndex = 0; buttonIndex < state.GetButtons().Count(); buttonIndex++)
            {
                if (state.GetButtons()[buttonIndex])
                {
                    _viewModel.AddGamepadBinding(-4, gamepadIndex, GetFocusedButton());
                }
            }
        }

        private Int32 GetFocusedButton()
        {
            if (System.Windows.Input.Keyboard.FocusedElement == p1up)
            {
                System.Windows.Input.Keyboard.Focus(p1down);
                return 0;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1down)
            {
                System.Windows.Input.Keyboard.Focus(p1left);
                return 1;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1left)
            {
                System.Windows.Input.Keyboard.Focus(p1right);
                return 2;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1right)
            {
                System.Windows.Input.Keyboard.Focus(p1a);
                return 3;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1a)
            {
                System.Windows.Input.Keyboard.Focus(p1b);
                return 4;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1b)
            {
                System.Windows.Input.Keyboard.Focus(p2up);
                return 5;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p2up)
            {
                System.Windows.Input.Keyboard.Focus(p2down);
                return 8;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p2down)
            {
                System.Windows.Input.Keyboard.Focus(p2left);
                return 9;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1left)
            {
                System.Windows.Input.Keyboard.Focus(p2right);
                return 10;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p1right)
            {
                System.Windows.Input.Keyboard.Focus(p2a);
                return 11;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p2a)
            {
                System.Windows.Input.Keyboard.Focus(p2b);
                return 12;
            }

            if (System.Windows.Input.Keyboard.FocusedElement == p2b)
            {
                System.Windows.Input.Keyboard.Focus(p1up);
                return 13;
            }

            return -1;
        }

        private void Clear_P1_Up(object sender, RoutedEventArgs e) { _viewModel.PlayerOneUp = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P1_Down(object sender, RoutedEventArgs e) { _viewModel.PlayerOneDown = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P1_Left(object sender, RoutedEventArgs e) { _viewModel.PlayerOneLeft = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P1_Right(object sender, RoutedEventArgs e) { _viewModel.PlayerOneRight = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P1_A(object sender, RoutedEventArgs e) { _viewModel.PlayerOneA = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P1_B(object sender, RoutedEventArgs e) { _viewModel.PlayerOneB = String.Empty; _viewModel.UpdateBindingsFromStrings(); }

        private void Clear_P2_Up(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoUp = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P2_Down(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoDown = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P2_Left(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoLeft = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P2_Right(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoRight = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P2_A(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoA = String.Empty; _viewModel.UpdateBindingsFromStrings(); }
        private void Clear_P2_B(object sender, RoutedEventArgs e) { _viewModel.PlayerTwoB = String.Empty; _viewModel.UpdateBindingsFromStrings(); }

        private void AddKey_P1_Up(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 0); System.Windows.Input.Keyboard.Focus(p1down); }
        private void AddKey_P1_Down(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 1); System.Windows.Input.Keyboard.Focus(p1left); }
        private void AddKey_P1_Left(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 2); System.Windows.Input.Keyboard.Focus(p1right); }
        private void AddKey_P1_Right(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 3); System.Windows.Input.Keyboard.Focus(p1a); }
        private void AddKey_P1_A(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 4); System.Windows.Input.Keyboard.Focus(p1b); }
        private void AddKey_P1_B(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 5); System.Windows.Input.Keyboard.Focus(p2up); }

        private void AddKey_P2_Up(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 8); System.Windows.Input.Keyboard.Focus(p2down); }
        private void AddKey_P2_Down(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 9); System.Windows.Input.Keyboard.Focus(p2left); }
        private void AddKey_P2_Left(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 10); System.Windows.Input.Keyboard.Focus(p2right); }
        private void AddKey_P2_Right(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 11); System.Windows.Input.Keyboard.Focus(p2a); }
        private void AddKey_P2_A(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 12); System.Windows.Input.Keyboard.Focus(p2b); }
        private void AddKey_P2_B(object sender, KeyEventArgs e) { _viewModel.AddKeyBinding(GetKey(e), 13); System.Windows.Input.Keyboard.Focus(p1up); }

        private System.Windows.Input.Key GetKey(KeyEventArgs e)
        {
            if (e.SystemKey != null)
                return e.SystemKey;

            return e.Key;
        }
    }
}
