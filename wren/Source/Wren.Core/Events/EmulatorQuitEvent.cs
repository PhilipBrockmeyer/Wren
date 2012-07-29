using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class EmulatorQuitEvent : IEvent
    {
        public Guid EmulationRunnerId { get; private set; }

        public EmulatorQuitEvent(Guid emulationRunnerId)
        {
            EmulationRunnerId = emulationRunnerId;
        }
    }
}
