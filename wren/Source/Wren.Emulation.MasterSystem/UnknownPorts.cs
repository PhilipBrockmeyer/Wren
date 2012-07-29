using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class UnknownPorts : IPortAddressableSystemComponent
    {
        public void RegisterPortAddresses(IPortManager portManager)
        {
            portManager.RegisterPort(0xDE, Write, Read);
            portManager.RegisterPort(0xDF, Write, Read);
            portManager.RegisterWritePort(0x3E, Write);
        }

        public Byte Read()
        {
            return 0;
        }

        public void Write(Byte value)
        {
        }
    }
}
