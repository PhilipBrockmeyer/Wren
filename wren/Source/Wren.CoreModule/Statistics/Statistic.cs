using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Wren.Core.GameEvents;

namespace Wren.Core.Statistics
{
    [Serializable]
    public abstract class Statistic<T> : IStatistic
        where T : IStatisticState
    {
        public String Description { get; set; }

        [XmlIgnore]
        public IStatisticState State { get; set; }
        public Guid Id { get; set; }

        [XmlIgnore]
        protected T DetailedState 
        {
            get { return (T)State; }
            set { State = value; }
        }

        public void Initialize(GameEventAggregator eventAggregator)
        {
            if (State == null)
            {
                State = Activator.CreateInstance<T>();
                State.StatisticDefinitionId = this.Id;
            }

            DoInitialize(eventAggregator);
        }

        public abstract void DoInitialize(GameEventAggregator eventAggregator);
    }
}
