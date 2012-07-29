using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Wren.Emulation.MasterSystem
{
    public class AddressManager : IAddressManager
    {
        ParameterExpression AddressParameter = Expression.Parameter(typeof(Int32), "address");
        ParameterExpression ValueParameter = Expression.Parameter(typeof(Byte), "value");
        MethodInfo _memoryWatchMethod = typeof(AddressManager).GetMethod("MemoryWatch");

        CacheManager _cacheManager;
        MethodInfo _invalidateCacheMethod;

        public ParameterExpression Address
        {
            get { return AddressParameter; }
        }

        public ParameterExpression Value
        {
            get { return ValueParameter; }
        }

        private Delegate _readDelegate;
        private Delegate ReadDelegate
        {
            get
            {
                if (_readDelegate == null)
                    _readDelegate = BuildReadDelegate();

                return _readDelegate;
            }
        }

        private Delegate _writeDelegate;
        private Delegate WriteDelegate
        {
            get
            {
                if (_writeDelegate == null)
                    _writeDelegate = BuildWriteDelegate();

                return _writeDelegate;
            }
        }

        IList<AddressBlock> _addressBlocks;

        public AddressManager(CacheManager cacheManager)
        {
            _addressBlocks = new List<AddressBlock>();
            _cacheManager = cacheManager;
            _invalidateCacheMethod = typeof(CacheManager).GetMethod("InvalidateRegion");
        }

        public void RegisterAddressBlock(AddressBlock addressBlock)
        {
            _readDelegate = null;
            _addressBlocks.Add(addressBlock);
        }
        
        public Byte ReadByte(Int32 address)
        {
            return (Byte)ReadDelegate.DynamicInvoke(address);
        }

        public void WriteByte(Int32 address, Byte value)
        {
            WriteDelegate.DynamicInvoke(address, value);
        }

        public Expression BuildAddressReadExpression(Expression address, Expression readResult)
        {
            IList<Expression> expressions = new List<Expression>();

            var orderedAddressBlocks = _addressBlocks.OrderBy(a => a.InclusiveLowerRange);

            foreach (var ab in orderedAddressBlocks)
            {               
                expressions.Add(
                    Expression.IfThen(Expression.GreaterThanOrEqual(address, Expression.Constant(ab.InclusiveLowerRange)),
                        Expression.IfThen(Expression.LessThanOrEqual(address, Expression.Constant(ab.InclusiveUpperRange)),
                            ab.ReadExpression(address, readResult)
                        )
                    )
                );
            }

            return Expression.Block(expressions);
        }

        public Expression BuildAddressWriteExpression(Expression address, Expression writeValue)
        {
            IList<Expression> expressions = new List<Expression>();

            var orderedAddressBlocks = _addressBlocks.OrderBy(a => a.InclusiveLowerRange);

            /*expressions.Add(
                Expression.IfThen(Expression.Equal(address, Expression.Constant(52344)),
                    Expression.Call(Expression.Constant(this), _memoryWatchMethod, address, writeValue)
                )
            );*/

            foreach (var ab in _addressBlocks)
            {
                expressions.Add(
                    Expression.IfThen(Expression.GreaterThanOrEqual(address, Expression.Constant(ab.InclusiveLowerRange)),
                        Expression.IfThen(Expression.LessThanOrEqual(address, Expression.Constant(ab.InclusiveUpperRange)),
                            ab.WriteExpression(address, writeValue, _invalidateCacheMethod, Expression.Constant(_cacheManager))
                        )
                    )
                );
            }

            return Expression.Block(expressions);
        }

        public void Clear()
        {
            _addressBlocks.Clear();
        }
               
        public Delegate BuildReadDelegate()
        {
            var expressions = new List<Expression>();
            expressions.Add(BuildAddressReadExpression(AddressParameter, ValueParameter));
            expressions.Add(ValueParameter);

            var parameterizedReadExpression = Expression.Block(new ParameterExpression[] { ValueParameter }, expressions);

            var lambda = Expression.Lambda(parameterizedReadExpression, Address);
            return lambda.Compile();
        }

        private Delegate BuildWriteDelegate()
        {
            var writeExpression = BuildAddressWriteExpression(AddressParameter, ValueParameter);

            var lambda = Expression.Lambda(writeExpression, Address, Value);
            return lambda.Compile();
        }

        public void MemoryWatch(Int32 address, Byte value)
        {
        }
    }
}
