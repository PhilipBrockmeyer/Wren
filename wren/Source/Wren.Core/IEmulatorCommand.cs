using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IEmulatorCommand
    {
        void Execute(IEmulator emulator);

    }
}
