using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wren.Core.Statistics
{
    [Serializable]
    public class TimerStatisticState : IStatisticState
    {
        public Int32 TimespanInMilliseconds { get; set; }

        [XmlIgnore]
        public TimeSpan EllapsedTime 
        {
            get { return TimeSpan.FromMilliseconds(TimespanInMilliseconds); }
            set { TimespanInMilliseconds = (Int32)value.TotalMilliseconds; }
        }

        public Guid StatisticDefinitionId { get; set; }
    }
}
