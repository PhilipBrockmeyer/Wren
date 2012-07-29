using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class Cartridge : IAddressableSystemComponent
    {
        public Byte[] _romData;
        public Int32 _page0Offset;
        public Int32 _page1Offset;
        public Int32 _page2Offset;
        public Boolean _useCartridgeRam;
        public Int32 _cartridgeRamPageOffset;
        
        FieldInfo _romDataField;
        FieldInfo _page0OffsetField;
        FieldInfo _page1OffsetField;
        FieldInfo _page2OffsetField;
        FieldInfo _userCartridgeRamField;

        
        ConstructorInfo _readOnlyConstructor;

        public Cartridge()
        {
            _romData = new Byte[512 * 1024];
            _romDataField = this.GetType().GetField("_romData");
        
            _page0Offset = 0x0000;
            _page1Offset = 0x4000;
            _page2Offset = 0x8000;

            _page0OffsetField = this.GetType().GetField("_page0Offset");
            _page1OffsetField = this.GetType().GetField("_page1Offset");
            _page2OffsetField = this.GetType().GetField("_page2Offset");
            _userCartridgeRamField = this.GetType().GetField("_useCartridgeRam");
                        
            _readOnlyConstructor = typeof(ReadOnlyException).GetConstructor(new Type[] { typeof(Int32), typeof(Byte) });
        }

        public void LoadRomData(Byte[] romData)
        {
            _romData = romData;
        }

        public void RegisterAddressBlocks(IAddressManager addressManager)
        {
            AddressBlock staticRomBank = new AddressBlock(0x0000, 0x03FF, true, true);
            AddressBlock romPage0 = new AddressBlock(0x0400, 0x3FFF, true, false);
            AddressBlock romPage1 = new AddressBlock(0x4000, 0x7FFF, true, false);
            AddressBlock romPage2 = new AddressBlock(0x8000, 0xBFFF, false, false);
            AddressBlock pageSelection = new AddressBlock(0xFFFC, 0xFFFF, false, true);

            staticRomBank.ReadExpression = StaticRomReadExpression;
            staticRomBank.WriteExpression = ReadOnlyExpression;

            romPage0.WriteExpression = ReadOnlyExpression;
            romPage0.ReadExpression  = RomPage0ReadExpression;
            
            romPage1.WriteExpression = ReadOnlyExpression;
            romPage1.ReadExpression =  RomPage1ReadExpression;
            
            romPage2.WriteExpression = ReadOnlyExpression;
            romPage2.ReadExpression = RomPage2ReadExpression;

            pageSelection.WriteExpression = PageSelectionWriteExpression;
            pageSelection.ReadExpression = WriteOnlyExpression;

            addressManager.RegisterAddressBlock(staticRomBank);
            addressManager.RegisterAddressBlock(romPage0);
            addressManager.RegisterAddressBlock(romPage1);
            addressManager.RegisterAddressBlock(romPage2);
            addressManager.RegisterAddressBlock(pageSelection);
        }

        public Int32 GetOffsetAddress(Int32 address)
        {
            if (address <= 0x03FF)
                return address;

            if (address <= 0x3FFF)
                return address + _page0Offset;

            if (address <= 0x7FFF)
                return address + _page1Offset;

            if (address <= 0xBFFF)
                return address + _page2Offset;

            throw new ApplicationException("Attempted to read an out of range address from the cartridge.");
        }

        public Expression WriteOnlyExpression(Expression address, Expression writeValue)
        {
            return Expression.Throw(Expression.New(_readOnlyConstructor, address, writeValue));
        }

        public Expression ReadOnlyExpression(Expression address, Expression writeValue, MethodInfo invalidateCacheMethod, Expression cacheManagerInstance)
        {
            return Expression.Throw(Expression.New(_readOnlyConstructor, address, writeValue));
        }

        public Expression StaticRomReadExpression(Expression address, Expression readResult)
        {
            return
                // readResult = _romData[address]
                Expression.Assign(readResult,
                    Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _romDataField), address)
                );
        }

        public Expression RomPage0ReadExpression(Expression address, Expression readResult)
        {
            return
                // readResult = rom[address + page0Offset]
                Expression.Assign(readResult,
                    Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _romDataField), 
                        Expression.Add( 
                            Expression.Field(Expression.Constant(this), _page0OffsetField),
                            address
                        )
                    )
                );
        }

        public Expression RomPage1ReadExpression(Expression address, Expression readResult)
        {
            return
                // readResult = rom[address + page0Offset - 0x4000]
                Expression.Assign(readResult,
                    Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _romDataField),
                        Expression.Subtract(
                            Expression.Add(
                                Expression.Field(Expression.Constant(this), _page1OffsetField),
                                address
                            ),
                            Expression.Constant(0x4000)
                        )
                    )
                );
        }

        public Expression RomPage2ReadExpression(Expression address, Expression readResult)
        {
            return
                // readResult = rom[address + page0Offset - 0x8000]
                Expression.Assign(readResult,
                    Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _romDataField),
                        Expression.Subtract(
                            Expression.Add(
                                Expression.Field(Expression.Constant(this), _page2OffsetField),
                                address
                            ),
                            Expression.Constant(0x8000)
                        )
                    )
                );
        }

        public Expression PageSelectionWriteExpression(Expression address, Expression writeValue, MethodInfo invalidateCacheMethod, Expression cacheManagerInstance)
        {
            return
                Expression.Switch(address,
                    // default.
                    ReadOnlyExpression(address, writeValue, invalidateCacheMethod, cacheManagerInstance),

                    // 0xFFFC
                    Expression.SwitchCase(
                        Expression.Block(
                            Expression.IfThenElse(Expression.NotEqual(Expression.Constant(0), Expression.And(address, Expression.Constant(0x08))),
                                Expression.Assign(Expression.Field(Expression.Constant(this), _userCartridgeRamField), Expression.Constant(true)),
                                Expression.Assign(Expression.Field(Expression.Constant(this), _userCartridgeRamField), Expression.Constant(false))
                            )
                        ),
                        Expression.Constant(0xFFFC)
                    ),

                    // 0xFFFD
                    Expression.SwitchCase(
                        Expression.Block(
                            Expression.Assign(
                                Expression.Field(Expression.Constant(this), _page0OffsetField),
                                Expression.Multiply(
                                    Expression.And(Expression.Convert(writeValue, typeof(Int32)), Expression.Constant(0x7F)),
                                    Expression.Constant(0x4000)
                                )
                            ),

                            Expression.Call(cacheManagerInstance, invalidateCacheMethod, Expression.Constant(0x0400), Expression.Constant(0x4000))
                        ),
                        Expression.Constant(0xFFFD)
                    ),

                    // 0xFFFE
                    Expression.SwitchCase(
                        Expression.Block(
                            Expression.Assign(
                                Expression.Field(Expression.Constant(this), _page1OffsetField),
                                Expression.Multiply(
                                    Expression.And(Expression.Convert(writeValue, typeof(Int32)), Expression.Constant(0x7F)),
                                    Expression.Constant(0x4000)
                                )
                            ),

                            Expression.Call(cacheManagerInstance, invalidateCacheMethod, Expression.Constant(0x0400), Expression.Constant(0x4000))
                        ),
                        Expression.Constant(0xFFFE)
                    ),

                    // 0xFFFF
                    Expression.SwitchCase(
                        Expression.Block(
                            Expression.Assign(
                                Expression.Field(Expression.Constant(this), _page2OffsetField),
                                Expression.Multiply(
                                    Expression.And(Expression.Convert(writeValue, typeof(Int32)), Expression.Constant(0x7F)),
                                    Expression.Constant(0x4000)
                                )
                            ),

                            Expression.Call(cacheManagerInstance, invalidateCacheMethod, Expression.Constant(0x8000), Expression.Constant(0x4000))
                        ),
                        Expression.Constant(0xFFFF)
                    )                       
                    
                );
        }

        public Byte ReadByte(Int32 address)
        {
            return _romData[address];
        }

        public void WriteByte(Int32 address, Byte value)
        {
            _romData[address] = value;
        }
    }
}
