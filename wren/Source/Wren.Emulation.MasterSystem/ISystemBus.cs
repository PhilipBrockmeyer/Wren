using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public interface ISystemBus
    {
        Byte ReadByte(Int32 address);
        void WriteByte(Int32 address, Byte value);

        Int32 ReadWord(Int32 address);
        void WriteWord(Int32 address, Int32 value);

        Byte ReadPort(Byte address);
        void WritePort(Byte address, Byte value);

        void RequestInterupt(Interupts interupt);
    }

    public interface ISystemBusExpressionProvider
    {
        Expression ReadByte(Expression address, Expression readResult);
        Expression WriteByte(Expression address, Expression value);
    }

    public interface IDebuggingSystemBus
    {
        void AddMemoryWatch(Int32 address);
        event EventHandler<ValueChangedEventArgs> ValueChanged;
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public Int32 Address { get; private set; }
        public Int32 Value { get; private set; }

        public ValueChangedEventArgs (Int32 address, Int32 value)
	    {
            Address = address;
            Value = value;
	    }        
    }
}
