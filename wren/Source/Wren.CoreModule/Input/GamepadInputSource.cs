using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using SlimDX.DirectInput;

namespace Wren.Core.Input
{
    public sealed class GamepadInputSource : IInputSource
    {
        Joystick _joystick;
        GamepadBindingSettings.GamepadBinding[] _bindings;

        public GamepadInputSource(Joystick joystick, GamepadBindingSettings.GamepadBinding[] bindings)
        {
            _joystick = joystick;
            _bindings = bindings;
        }

        public InputState GetCurrentInputState()
        {
            InputState inputState = new InputState();

            if (_joystick.Acquire().IsFailure)
                return inputState;

            if (_joystick.Poll().IsFailure)
                return inputState;

            JoystickState joystickState = _joystick.GetCurrentState();

            foreach (var binding in _bindings)
            {
                if (binding.Button < 0)
                {
                    switch (binding.Button)
                    {
                        case -1:
                            if (joystickState.Y < 0x4000)
                                inputState.SetButtonState(binding.ButtonId, true);
                            break;
                        case  -2:
                            if (joystickState.Y > 0xC000)
                                inputState.SetButtonState(binding.ButtonId, true);
                            break;
                        case -3:
                            if (joystickState.X < 0x4000)
                                inputState.SetButtonState(binding.ButtonId, true);
                            break;
                        case -4:
                            if (joystickState.X > 0xC000)
                                inputState.SetButtonState(binding.ButtonId, true);
                            break;
    
                        default:
                            break;
                    }
                }
                else
                {
                    if (joystickState.IsPressed(binding.Button))
                        inputState.SetButtonState(binding.ButtonId, true);
                }
            }

            return inputState;
        }

        public void Open()
        { }

        public void Close()
        { }

    }
}
