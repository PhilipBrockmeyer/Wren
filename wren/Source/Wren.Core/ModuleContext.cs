using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class ModuleContext : IModuleContext
    {
        public IServiceLocator ServiceLocator { get; set; }
        public IInputSourceAssembler InputSourceAssembler { get; set; }
        public IEmulatorRegistry EmulatorRegistry { get; set; }

    }
}
