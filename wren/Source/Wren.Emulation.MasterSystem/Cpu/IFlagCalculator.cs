using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public interface IFlagCalculator
    {
        Boolean SignFlag { get; }
        Boolean ZeroFlag { get; }
        Boolean Flag5 { get; }
        Boolean HalfCarryFlag { get; }
        Boolean Flag3 { get; }
        Boolean ParityOverflowFlag { get; }
        Boolean NegativeFlag { get; }
        Boolean CarryFlag { get; }
        Int32 AllFlags { get; }
    }
}
