using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public sealed class ArraySystemBus : ISystemBus
    {
        public Byte[] memory;
        private int p;

        public ArraySystemBus(Int32 memorySize)
        {
            memory = new Byte[memorySize];
        }

        public Byte ReadByte(Int32 address)
        {
            return memory[address];
        }

        public void WriteByte(Int32 address, Byte value)
        {
            memory[address] = (Byte)(value & 0xFF);
        }

        public int ReadWord(int address)
        {
            throw new NotImplementedException();
        }

        public void WriteWord(int address, int value)
        {
            throw new NotImplementedException();
        }

        public void RequestInterupt(Interupts interupt)
        {
            throw new NotImplementedException();
        }


        public byte ReadPort(byte address)
        {
            throw new NotImplementedException();
        }

        public void WritePort(byte address, byte value)
        {
            throw new NotImplementedException();
        }
    }
}
