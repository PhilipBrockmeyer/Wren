using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Persistence;

namespace Wren.Core.Replay
{
    public sealed class RecordingInputSourceDecorator : IInputSource
    {
        Int32 _frame;
        IInputSource _source;
        String _filePath;
        IPersistenceManager _persistenceManager;
        ReplayData _data;
        InputState _previousState;
        Int32 _buttonCount;

        public RecordingInputSourceDecorator(IInputSource source, String filePath, IPersistenceManager persistenceManager, Int32 buttonCount)
        {
            _frame = 0;
            _source = source;
            _filePath = filePath;
            _persistenceManager = persistenceManager;
            _previousState = new InputState();
            _data = new ReplayData();
            _buttonCount = buttonCount;
        }

        public void Open()
        {
            _source.Open();
        }

        public void Close()
        {
            _source.Close();

            _persistenceManager.Save(_filePath, ReplayModule.ReplayPersistenceProviderKey, _data);
        }

        public InputState GetCurrentInputState()
        {
            var state = _source.GetCurrentInputState();

            for (Int32 i = 0; i < _buttonCount; i++)
            {
                if (state.GetIsButtonPresed(i) != _previousState.GetIsButtonPresed(i))
                {
                    if (state.GetIsButtonPresed(i))
                        _data.AddButtonAction(ReplayData.ButtonAction.Pressed, i, _frame);
                    else
                        _data.AddButtonAction(ReplayData.ButtonAction.Released, i, _frame);
                }
            }

            _previousState = state;
            _frame++;
            return state;
        }
    }
}
