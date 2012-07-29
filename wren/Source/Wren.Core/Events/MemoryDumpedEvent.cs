using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;

namespace Wren.Core
{
    public class MemoryDumpedEvent : IEvent
    {
        public Byte[] MemoryDump { get; private set; }
        public Int32 BaseAddress { get; private set; }

        public MemoryDumpedEvent(Byte[] memoryDump, Int32 baseAddress)
        {
            if (memoryDump == null)
                throw new ApplicationException();

            MemoryDump = memoryDump;
            BaseAddress = baseAddress;
        }
    }
}
