using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Statistics;

namespace Wren.Module.Core.Statistics
{
    [Serializable]
    public class NumericStatisticState : IStatisticState
    {
        public Guid StatisticDefinitionId { get; set; }
        public Int32 Value { get; set; }
    }
}
