using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class CpuContext : ICpuContext
    {
        public CpuData Data { get; set; }
        public ISystemBus SystemBus { get; set; }
        public FlagTables Flags { get; set; }
        public Int32 CycleCounter { get; set; }
        public BreakpointHandler BreakpointHandler { get; set; }
    }
}
