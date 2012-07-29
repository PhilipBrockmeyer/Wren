using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Debugging
{
    public class MemoryDumpCommand : IEmulatorCommand
    {
        public void Execute(IEmulator emulator)
        {
            var debugger = emulator as IDebuggingEmulator;

            if (debugger == null)
                return;

            debugger.DumpMemory();
        }
    }
}
