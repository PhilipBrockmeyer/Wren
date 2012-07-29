using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class PortManager : IPortManager
    {
        IDictionary<Byte, PortWriteHandler> _writeHandlers;
        IDictionary<Byte, PortReadHandler> _readHandlers;

        public PortManager()
        {
            _writeHandlers = new Dictionary<Byte, PortWriteHandler>();
            _readHandlers = new Dictionary<Byte, PortReadHandler>();
        }

        public void RegisterPort(Byte port, PortWriteHandler writeHandler, PortReadHandler readHandler)
        {
            _writeHandlers[port] = writeHandler;
            _readHandlers[port] = readHandler;
        }

        public void RegisterWritePort(Byte port, PortWriteHandler writeHandler)
        {
            _writeHandlers[port] = writeHandler;
        }

        public void RegisterReadPort(Byte port, PortReadHandler readHandler)
        {
            _readHandlers[port] = readHandler;
        }

        public void WritePort(Byte port, Byte value)
        {
            _writeHandlers[port](value);
        }

        public Byte ReadPort(Byte port)
        {
            return _readHandlers[port]();
        }

        public void Clear()
        {
            _writeHandlers.Clear();
            _readHandlers.Clear();
        }
    }
}
