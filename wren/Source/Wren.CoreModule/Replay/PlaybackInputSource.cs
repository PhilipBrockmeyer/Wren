using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Replay;

namespace Wren.Core.Replay
{
    public class PlaybackInputSource : IInputSource
    {
        Wren.Core.Replay.ReplayData.KeyData[] _data;
        IList<Boolean> _buttons;
        Int32 _buttonCount;
        Int32 _frame;
        Int32 _frameIndex;
        Int32 _nextInputFrame;

        public PlaybackInputSource(ReplayData data, Int32 buttonCount)
        {
            _data = data.Data;
            _buttons = new List<Boolean>();
            _buttonCount = buttonCount;
            _frame = 0;
            _frameIndex = 0;

            if (_data.Length > 0)
                _nextInputFrame = _data[0].Frame;
            else
                _nextInputFrame = -1;

            for (Int32 i = 0; i < buttonCount; i++)
                _buttons.Add(false);
                  
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public InputState GetCurrentInputState()
        {
            var input = new InputState();

            while (_frame == _nextInputFrame)
            {
                var action = _data[_frameIndex];

                if (action.Action == 0)
                    _buttons[action.ButtonId] = true;
                else
                    _buttons[action.ButtonId] = false;

                _frameIndex++;


                if (_frameIndex < _data.Count())
                    _nextInputFrame = _data[_frameIndex].Frame;
                else
                    _nextInputFrame = -1;
            }

            for (Int32 i = 0; i < _buttonCount; i++)
            {
                input.SetButtonState(i, _buttons[i]);
            }

            _frame++;

            return input;
        }
    }
}
