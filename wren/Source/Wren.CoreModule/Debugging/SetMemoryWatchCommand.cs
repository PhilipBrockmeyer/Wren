using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Debugging
{
    public class SetMemoryWatchCommand : IEmulatorCommand
    {
        public Int32 MemoryAddress { get; private set; }

        public SetMemoryWatchCommand(Int32 memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public void Execute(IEmulator emulator)
        {
            var debugger = emulator as IDebuggingEmulator;

            if (debugger == null)
                return;

            debugger.SetMemoryWatch8(MemoryAddress);
        }
    }
}
