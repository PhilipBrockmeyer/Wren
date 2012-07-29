using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Emulation.Nes;

namespace Wren.Nes
{
    public class NesModule : IModule
    {
        public void Load(IModuleContext context)
        {
            context.EmulatorRegistry.RegisterEmulator<NesEmulator>(new EmulatedSystem("NES"));
        }

        public void Unload(IModuleContext context)
        {
            
        }
    }
}
