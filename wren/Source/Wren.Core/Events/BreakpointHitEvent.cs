using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class BreakpointHitEvent : IEvent
    {
        public Int32 Address { get; private set; }

        public BreakpointHitEvent(Int32 address)
        {
            Address = address;
        }
    }
}
