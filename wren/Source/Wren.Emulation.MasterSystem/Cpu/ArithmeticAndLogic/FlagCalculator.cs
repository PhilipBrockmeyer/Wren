using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.ArithmeticAndLogic
{
    public abstract class FlagCalculator : IFlagCalculator
    {
        public abstract Boolean SignFlag { get; }
        public abstract Boolean ZeroFlag { get; }
        public abstract Boolean Flag5 { get; }
        public abstract Boolean HalfCarryFlag { get; }
        public abstract Boolean Flag3 { get; }
        public abstract Boolean ParityOverflowFlag { get; }
        public abstract Boolean NegativeFlag { get; }
        public abstract Boolean CarryFlag { get; }

        public Int32 AllFlags
        {
            get
            {
                return (ZeroFlag ? Flags.ZeroFlag : 0)
                    | (SignFlag ? Flags.SignFlag : 0)
                    | (Flag5 ? Flags.Flag5 : 0)
                    | (HalfCarryFlag ? Flags.HalfCarry : 0)
                    | (Flag3 ? Flags.Flag3 : 0)
                    | (ParityOverflowFlag ? Flags.OverflowFlag : 0)
                    | (NegativeFlag ? Flags.SubtractionFlag : 0)
                    | (CarryFlag ? Flags.CarryFlag : 0);
            }
        }
    }
}
