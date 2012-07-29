using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.GameEvents;

namespace Wren.Core.Statistics
{
    public interface IStatistic
    {
        String Description { get; set; }
        Guid Id { get; set; }
        IStatisticState State { get; set; }
        void Initialize(GameEventAggregator gameEventAggregator);
    }
}
