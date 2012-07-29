using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public interface ICpuContext
    {
        CpuData Data { get; set; }
        ISystemBus SystemBus { get; }
        FlagTables Flags { get; }
        BreakpointHandler BreakpointHandler { get; }
    }
}
