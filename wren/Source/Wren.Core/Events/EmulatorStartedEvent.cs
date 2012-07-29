using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class EmulatorStartedEvent : IEvent
    {
        public Guid EmulationRunnerId { get; set; }

        public EmulatorStartedEvent(Guid emulationRunnerId)
        {
            EmulationRunnerId = emulationRunnerId;
        }
    }
}
