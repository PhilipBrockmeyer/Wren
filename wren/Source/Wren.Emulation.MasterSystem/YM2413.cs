using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class YM2413 : IPortAddressableSystemComponent
    {
        public void RegisterPortAddresses(IPortManager portManager)
        {
            portManager.RegisterWritePort(0xF0, value => value = 1);
            portManager.RegisterWritePort(0xF1, value => value = 1);
            portManager.RegisterPort(0xF2, value => value = 1, () => 0xFF);
        }
    }
}
