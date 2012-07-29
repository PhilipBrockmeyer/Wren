using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IModuleContext
    {
        IServiceLocator ServiceLocator { get; }
        IInputSourceAssembler InputSourceAssembler { get; }
        IEmulatorRegistry EmulatorRegistry { get; }
    }
}
