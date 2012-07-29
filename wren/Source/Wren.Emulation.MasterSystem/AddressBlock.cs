using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Wren.Emulation.MasterSystem
{
    public delegate Expression AddressBlockReadExpression(Expression address, Expression readResult);
    public delegate Expression AddressBlockWriteExpression(Expression address, Expression writeValue, MethodInfo invalidateCacheMethod, Expression cacheManagerInstance);
    
    public sealed class AddressBlock
    {
        public Int32 InclusiveLowerRange { get; private set; }
        public Int32 InclusiveUpperRange { get; private set; }
        public Boolean IsReadOnly { get; private set; }
        public Boolean IsStaticallyAddressable { get; private set; }
        public AddressBlockReadExpression ReadExpression { get; set; }
        public AddressBlockWriteExpression WriteExpression { get; set; }

        public AddressBlock(Int32 inclusiveLowerRange, 
                            Int32 inclusiveUpperRange, 
                            Boolean isReadOnly, 
                            Boolean isStaticallyAddressable)
        {
            InclusiveLowerRange = inclusiveLowerRange;
            InclusiveUpperRange = inclusiveUpperRange;
            IsReadOnly = isReadOnly;
            IsStaticallyAddressable = isStaticallyAddressable;
        }
    }
}
