using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public static class Flags
    {
        public const Int32 CarryFlag = 0x01;
        public const Int32 SubtractionFlag = 0x02;
        public const Int32 OverflowFlag = 0x04;
        public const Int32 ParityFlag = 0x04;
        public const Int32 Flag3 = 0x08;
        public const Int32 HalfCarry = 0x10;
        public const Int32 Flag5 = 0x20;
        public const Int32 ZeroFlag = 0x40;
        public const Int32 SignFlag = 0x80;

        public static Boolean ReadFlag(Int32 flagsReg, Int32 flagId)
        {
            return (flagsReg & flagId) != 0;
        }
    }
}
