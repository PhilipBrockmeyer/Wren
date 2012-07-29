using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class CpuData
    {
        public Int32 RegisterA;
        public Int32 RegisterB;
        public Int32 RegisterC;
        public Int32 RegisterD;
        public Int32 RegisterE;
        public Int32 RegisterH;
        public Int32 RegisterL;

        public Int32 RegisterIX;
        public Int32 RegisterIY;

        public Int32 FlagsRegister;
        public Int32 StackPointerRegister;
        public Int32 ProgramCounterRegister;

        public Int32 RegisterAPrime;
        public Int32 RegisterFPrime;
        public Int32 RegisterBPrime;
        public Int32 RegisterCPrime;
        public Int32 RegisterDPrime;
        public Int32 RegisterEPrime;
        public Int32 RegisterHPrime;
        public Int32 RegisterLPrime;

        public Boolean IsHalted;
        public Int32 CycleCounter;

        public Int32 InteruptVectorRegister;
        public Boolean IFF1;
        public Boolean IFF2;
        public Int32 InteruptMode;
        public Boolean InteruptRequested;
        public Int32 RefreshRegister;

        public Int32[] CpuCache;

        public CpuData()
        {
            CpuCache = new Int32[0x12000];

            for (Int32 i = 0; i < 0x12000; i++)
                CpuCache[i] = -1;
        }
    }
}
