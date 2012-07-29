using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using SlimDX.DirectInput;

namespace Wren.Core.Input
{
    public sealed class KeyboardInputSource : IInputSource
    {
        Keyboard _keyboard;
        KeyboardBindingSettings _bindings;

        public KeyboardInputSource(Keyboard keyboard, KeyboardBindingSettings bindings)
        {
            _keyboard = keyboard;
            _bindings = bindings;
        }

        public InputState GetCurrentInputState()
        {
            InputState inputState = new InputState();

            if (_keyboard.Acquire().IsFailure)
                return inputState;

            if (_keyboard.Poll().IsFailure)
                return inputState;

            KeyboardState keyboardState = _keyboard.GetCurrentState();

            foreach (var binding in _bindings.Bindings)
            {
                if (keyboardState.IsPressed((Key)Enum.Parse(typeof(Key), binding.Key)))
                    inputState.SetButtonState(binding.ButtonId, true);
            }

            return inputState;
        }

        public void Open()
        { }

        public void Close()
        { }

    }
}
