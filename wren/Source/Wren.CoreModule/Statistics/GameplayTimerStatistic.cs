using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Events;
using Wren.Core.GameEvents;

namespace Wren.Core.Statistics
{
    [Serializable]
    public class GameplayTimerStatistic : Statistic<TimerStatisticState>
    {
        DateTime _gameplayStarted;

        public GameplayTimerStatistic()
        {
         
        }

        public override void DoInitialize(GameEventAggregator eventAggregator)
        {
            eventAggregator.AddGameStartedHandler(EmulatorStarted);
            eventAggregator.AddGameEndedHandler(EmulatorQuit);
        }

        protected void EmulatorStarted()
        {
            _gameplayStarted = DateTime.Now;
        }

        protected void EmulatorQuit()
        {
            DetailedState.EllapsedTime = DetailedState.EllapsedTime.Add(DateTime.Now.Subtract(_gameplayStarted));
        }
    }
}
