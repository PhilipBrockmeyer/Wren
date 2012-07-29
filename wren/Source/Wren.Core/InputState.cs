using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class InputState
    {
        IDictionary<Int32, Boolean> _buttons;

        public InputState()
        {
            _buttons = new Dictionary<Int32, Boolean>();
        }

        public Boolean GetIsButtonPresed(Int32 buttonId)
        {
            if (!_buttons.ContainsKey(buttonId))
                return false;

            return _buttons[buttonId];
        }

        public void SetButtonState(Int32 buttonId, Boolean isPressed)
        {
            if (!_buttons.ContainsKey(buttonId))
                _buttons.Add(buttonId, isPressed);

            _buttons[buttonId] = isPressed;
        }

        public void Merge(InputState inputState)
        {
            foreach (var button in inputState._buttons)
            {
                if (button.Value)
                {
                    SetButtonState(button.Key, true);
                }
            }
        }
    }
}
