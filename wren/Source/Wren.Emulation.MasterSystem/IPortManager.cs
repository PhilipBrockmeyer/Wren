using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public delegate void PortWriteHandler(Byte value);
    public delegate Byte PortReadHandler();

    public interface IPortManager
    {
        void RegisterPort(Byte port, PortWriteHandler writeHandler, PortReadHandler readHandler);
        void RegisterWritePort(Byte port, PortWriteHandler writeHandler);
        void RegisterReadPort(Byte port, PortReadHandler readHandler);
        void WritePort(Byte port, Byte value);
        Byte ReadPort(Byte port);

        void Clear();
    }
}
