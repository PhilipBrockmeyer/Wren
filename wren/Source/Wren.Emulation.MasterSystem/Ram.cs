using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;

namespace Wren.Emulation.MasterSystem
{
    public class Ram : IAddressableSystemComponent, IStatefulSystemComponent
    {
        public Byte[] _memory = new Byte[0x2000 * 2];
        FieldInfo _memoryField;

        public Ram()
        {
            _memoryField = this.GetType().GetField("_memory");
        }

        public void RegisterAddressBlocks(IAddressManager addressManager)
        {
            var block = new AddressBlock(0xC000, 0xFFFB, false, false);

            block.ReadExpression = Read;
            block.WriteExpression = Write;
                
            addressManager.RegisterAddressBlock(block);
        }

        private Expression Read(Expression address, Expression readResult)
        {
            return Expression.Block(
                // readResult = _memory[address - 0xC000]
                Expression.Assign(readResult,
                    Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _memoryField),
                        Expression.Subtract(
                            address,
                            Expression.Constant(0xC000)
                        )
                    )
                )
            );
        }

        private Expression Write(Expression address, Expression writeValue, MethodInfo invalidateCacheMethod, Expression cacheManagerInstance)
        {
            return Expression.Block(
                // _memory[address - 0xC000] = value
                Expression.Assign(
                    Expression.ArrayAccess(Expression.Field(Expression.Constant(this), _memoryField),
                        Expression.Subtract(address, Expression.Constant(0xC000))
                    ),

                    writeValue
                ),

                // Mirror [C000..DFFF] into [E000..FFFF]
                Expression.IfThenElse(Expression.LessThan(address, Expression.Constant(0xE000)),

                    // _memory[address - (0xC000) + 0x2000] = value
                    Expression.Assign(
                        Expression.ArrayAccess(Expression.Field(Expression.Constant(this), _memoryField),
                            Expression.Subtract(address, Expression.Constant(0xA000))
                        ),
                        writeValue
                    ),

                    // _memory[address - (0xC000 + 0x2000)] = value
                    Expression.Assign(
                        Expression.ArrayAccess(Expression.Field(Expression.Constant(this), _memoryField),
                            Expression.Subtract(address, Expression.Constant(0xE000))
                        ),
                        writeValue
                    )
                )
            );
        }

        public void SerializeComponentState(BinaryWriter writer)
        {
            writer.Write(_memory);
        }

        public void DeserializeComponentState(BinaryReader reader)
        {
            _memory = reader.ReadBytes(_memory.Length);
        }
    }
}
