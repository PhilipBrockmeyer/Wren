using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;

namespace Wren.Core.Events
{
    public class MemoryValueChangedEvent : IEvent
    {
        public Int32 Address { get; private set; }
        public Int32 NewValue { get; private set; }

        public MemoryValueChangedEvent(Int32 address, Int32 newValue)
        {
            Address = address;
            NewValue = newValue;
        }
    }
}
