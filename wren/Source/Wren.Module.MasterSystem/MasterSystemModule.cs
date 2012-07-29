using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Emulation.MasterSystem;

namespace Wren.Nes
{
    public class NesModule : IModule
    {
        public void Load(IModuleContext context)
        {
            context.EmulatorRegistry.RegisterEmulator<SmsEmulator>(new EmulatedSystem("SMS"));
        }

        public void Unload(IModuleContext context)
        {
            
        }
    }
}
