using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Statistics
{
    public interface IStatisticState
    {
        Guid StatisticDefinitionId { get; set; }
    }
}
