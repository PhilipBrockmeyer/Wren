using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Debugging;
using Wren.Core.Events;

namespace Wren.Core.GameEvents
{
    public class GameEventAggregator
    {
        IEventAggregator _eventAggregator;
        IEmulationRunner _emulationRunner;
        IDictionary<Int32, IList<Action<GameMemoryChangedEvent>>> _memoryChangedActions;
        IList<Action> _gameStartedActions;
        IList<Action> _gameEndedActions;

        public GameEventAggregator(IEventAggregator eventAggregator, IEmulationRunner emulationRunner)
        {
            _eventAggregator = eventAggregator;
            _emulationRunner = emulationRunner;
            _gameStartedActions = new List<Action>();
            _gameEndedActions = new List<Action>();
            _memoryChangedActions = new Dictionary<Int32, IList<Action<GameMemoryChangedEvent>>>();

            eventAggregator.Subscribe<MemoryValueChangedEvent>(ValueChanged);
            eventAggregator.Subscribe<EmulatorStartedEvent>(EmulatorStarted);
            eventAggregator.Subscribe<EmulatorQuitEvent>(EmulatorQuit);
        }

        public void AddMemoryWatch(Int32 memoryAddress, Action<GameMemoryChangedEvent> action)
        {
            _emulationRunner.SendCommand(new SetMemoryWatchCommand(memoryAddress));

            if (!_memoryChangedActions.ContainsKey(memoryAddress))
                _memoryChangedActions.Add(memoryAddress, new List<Action<GameMemoryChangedEvent>>());

            _memoryChangedActions[memoryAddress].Add(action);
        }

        public void AddGameStartedHandler(Action handler)
        {
            _gameStartedActions.Add(handler);
        }

        public void AddGameEndedHandler(Action handler)
        {
            _gameEndedActions.Add(handler);
        }

        public void EmulatorStarted(IEvent e)
        {
            foreach (var a in _gameStartedActions)
            {
                a.Invoke();
            }
        }

        public void EmulatorQuit(IEvent e)
        {
            foreach (var a in _gameEndedActions)
            {
                a.Invoke();
            }
        }

        public void ValueChanged(IEvent e)
        {
            MemoryValueChangedEvent mvce = e as MemoryValueChangedEvent;
            GameMemoryChangedEvent gmce = new GameMemoryChangedEvent(mvce.Address, mvce.NewValue);

            foreach (var a in _memoryChangedActions[mvce.Address])
            {
                a.Invoke(gmce);
            }
        }
    }
}
