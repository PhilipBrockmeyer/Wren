using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public interface IAddressManager
    {
        ParameterExpression Address { get; }
        ParameterExpression Value { get; }
        
        void RegisterAddressBlock(AddressBlock addressBlock);

        Byte ReadByte(Int32 address);
        void WriteByte(Int32 address, Byte value);

        Expression BuildAddressReadExpression(Expression address, Expression readResult);
        Expression BuildAddressWriteExpression(Expression address, Expression writeValue);

        void Clear();
    }
}
