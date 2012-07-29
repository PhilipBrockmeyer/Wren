using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class BreakpointHandler : IAddressableSystemComponent
    {
        public Byte[] interuptProgram;
        public Int32 instructionSize;

        Int32 _interuptAddressPointer;
        Cartridge _cartridge;
        IDictionary<Int32, Breakpoint> _breakpoints;

        ConstructorInfo _readOnlyConstructor = typeof(ReadOnlyException).GetConstructor(new Type[] { typeof(Int32), typeof(Byte) });
        FieldInfo _instructionSizeField = typeof(BreakpointHandler).GetField("instructionSize");
        FieldInfo _interuptProgramArray = typeof(BreakpointHandler).GetField("interuptProgram");

        public BreakpointHandler(Cartridge cartridge)
        {
            _cartridge = cartridge;
            _breakpoints = new Dictionary<Int32, Breakpoint>();
            interuptProgram = new Byte[1000];
        }
        
        public void RegisterAddressBlocks(IAddressManager addressManager)
        {
            AddressBlock addressBlock = new AddressBlock(0x10000, 0x10006, false, false);
            addressBlock.ReadExpression = ReadExpression;
            addressBlock.WriteExpression = WriteExpression;
            addressManager.RegisterAddressBlock(addressBlock);
        }

        public void AddBreakpoint(Int32 address, Int32 instructionSize)
        {
            _breakpoints.Add(address, new Breakpoint() { OriginalByte = _cartridge.ReadByte(address), InstructionSize = instructionSize, InteruptAddress = _interuptAddressPointer });
            
            // copy all instruction bytes into the interupt program.
            for (Int32 i = 0; i < instructionSize; i++)
            {
                interuptProgram[_interuptAddressPointer + i] = _cartridge.ReadByte(address + i);
            }

            // add return instruction.
            interuptProgram[_interuptAddressPointer + instructionSize] = 0xC9;

            // Set the pointer to the position for the next interupt.
            _interuptAddressPointer += instructionSize;
            _interuptAddressPointer++;

            _cartridge.WriteByte(address, 0xFF);
        }

        public Int32 GetInstructionSize(Int32 address)
        {
            var b = _breakpoints[_cartridge.GetOffsetAddress(address)];
            return b.InstructionSize;
        }

        public Int32 GetInteruptAddress(Int32 address)
        {
            var b = _breakpoints[_cartridge.GetOffsetAddress(address)];
            return b.InteruptAddress;
        }

        public void HandleBreakpoint(Int32 address)
        {
            var b = _breakpoints[_cartridge.GetOffsetAddress(address)];
            OnBreakpointHit(new BreakpointHitEventArgs(_cartridge.GetOffsetAddress(address)));
        }

        public Boolean IsBreakpoint(Int32 address)
        {
            if (!_breakpoints.ContainsKey(_cartridge.GetOffsetAddress(address)))
                return false;

            return true;
        }

        public Expression ReadExpression(Expression address, Expression readResult)
        {
            return 
                Expression.Block(
                    Expression.Assign(readResult, 
                        Expression.ArrayIndex(Expression.Field(Expression.Constant(this), _interuptProgramArray), address)
                    )
                );
        }

        public Expression WriteExpression(Expression address, Expression writeValue, MethodInfo invalidateCacheMethod, Expression cacheManagerInstance)
        {
            return Expression.Throw(Expression.New(_readOnlyConstructor, address, writeValue));
        }

        protected virtual void OnBreakpointHit(BreakpointHitEventArgs e)
        {
            if (BreakpointHit != null)
                BreakpointHit(this, e);
        }

        public event EventHandler<BreakpointHitEventArgs> BreakpointHit;

    }

    public class Breakpoint
    {
        public Byte OriginalByte { get; set; }
        public Int32 InstructionSize { get; set; }
        public Int32 InteruptAddress { get; set; }
    }

    public class BreakpointHitEventArgs : EventArgs
    {
        public Int32 Address { get; private set; }

        public BreakpointHitEventArgs(Int32 address)
        {
            Address = address;
        }
    }
}
