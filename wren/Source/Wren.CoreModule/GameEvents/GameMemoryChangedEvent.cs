using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.GameEvents
{
    public class GameMemoryChangedEvent
    {
        public Int32 Address { get; private set; }
        public Int32 NewValue { get; private set; }

        public GameMemoryChangedEvent(Int32 address, Int32 newValue)
        {
            Address = address;
            NewValue = newValue;
        }
    }
}
