using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public class SystemBus : ISystemBus, IDebuggingSystemBus //, ISystemBusExpressionProvider
    {
        IInteruptManager _interuptManager;
        IAddressManager _addressManager;
        IPortManager _portManager;
        Delegate _readDelegate;
        IList<Int32> _watchList;

        public SystemBus(IInteruptManager interuptManager, IAddressManager addressManager, IPortManager portManager)
        {
            _interuptManager = interuptManager;
            _addressManager = addressManager;
            _portManager = portManager;
            _watchList = new List<Int32>();
        }

        public Byte ReadByte(Int32 address)
        {
            return _addressManager.ReadByte(address);
        }

        public void WriteByte(Int32 address, Byte value)
        {
            if (_watchList.Contains(address))
                OnValueChanged(new ValueChangedEventArgs(address, value));

            _addressManager.WriteByte(address, value);
        }

        public Int32 ReadWord(Int32 address)
        {
            return _addressManager.ReadByte(address) | (_addressManager.ReadByte(address + 1) << 8);
        }

        public void WriteWord(Int32 address, Int32 value)
        {
            if (_watchList.Contains(address) || _watchList.Contains(address + 1))
                OnValueChanged(new ValueChangedEventArgs(address, value));

            _addressManager.WriteByte(address, (Byte)(value & 0xFF));
            _addressManager.WriteByte(address + 1, (Byte)(value >> 8));
        }

        public void RequestInterupt(Interupts interupt)
        {
            _interuptManager.RunInteruptHandlers(interupt);
        }

        public Byte ReadPort(Byte address)
        {
            return _portManager.ReadPort(address);
        }

        public void WritePort(Byte address, Byte value)
        {
            _portManager.WritePort(address, value);
        }

        public Expression ReadByte(Expression address, Expression readResult)
        {
            return _addressManager.BuildAddressReadExpression(address, readResult);
        }

        public Expression WriteByte(Expression address, Expression value)
        {
            return _addressManager.BuildAddressWriteExpression(address, value);
        }

        public void AddMemoryWatch(Int32 address)
        {
            if (!_watchList.Contains(address))
                _watchList.Add(address);
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
}
