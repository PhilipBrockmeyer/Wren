using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IDebuggingEmulator
    {
        void DumpMemory();
        void SetMemoryWatch8(Int32 memoryAddress);
        void SetBreakPoint(Int32 address);
        Int32 ReadMemory(Int32 address);
    }
}
