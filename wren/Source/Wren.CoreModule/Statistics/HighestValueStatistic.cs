using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Module.Core.Statistics;
using Wren.Core.Debugging;
using Wren.Core.GameEvents;

namespace Wren.Core.Statistics
{
    [Serializable]
    public class HighestValueStatistic : Statistic<NumericStatisticState>
    {
        public Int32 MemoryAddress { get; set; }

        public override void DoInitialize(GameEventAggregator eventAggregator)
        {
            eventAggregator.AddMemoryWatch(MemoryAddress, ValueChanged);
        }

        protected void ValueChanged(GameMemoryChangedEvent e)
        {
            if (e.NewValue > DetailedState.Value)
                DetailedState.Value = e.NewValue;
        }
    }
}
