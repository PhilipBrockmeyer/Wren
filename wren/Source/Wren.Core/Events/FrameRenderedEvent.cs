using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class FrameRenderedEvent : IEvent
    {
        public Guid EmulationRunnerId { get; private set; }

        public FrameRenderedEvent(Guid emulationRunnerId)
        {
            EmulationRunnerId = emulationRunnerId;
        }
    }
}
