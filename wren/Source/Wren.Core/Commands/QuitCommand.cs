using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Commands
{
    public class QuitCommand : IEmulatorCommand
    {
        public void Execute(IEmulator emulator)
        {
            emulator.Quit();
        }
    }
}
