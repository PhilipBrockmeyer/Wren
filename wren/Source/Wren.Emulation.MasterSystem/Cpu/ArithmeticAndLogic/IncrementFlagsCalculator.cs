using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.ArithmeticAndLogic
{
    public class IncrementFlagsCalculator : FlagCalculator
    {
        Int32 _value;  // the number to increment.

        public IncrementFlagsCalculator(Int32 value)
        {
            _value = value;
        }

        public override bool SignFlag
        {
            get { return ((_value + 1) & 0x80) == 0x80; }
        }

        public override bool ZeroFlag
        {
            get { return _value == 0xFF; }
        }

        public override bool Flag5
        {
            get { return ((_value + 1) & Flags.Flag5) == Flags.Flag5; }
        }

        public override bool HalfCarryFlag
        {
            get { return (_value & 0x0F) == 0x0F; }
        }

        public override bool Flag3
        {
            get { return ((_value + 1) & Flags.Flag3) == Flags.Flag3; }
        }

        public override bool ParityOverflowFlag
        {
            get { return _value == 0x7F; }
        }

        public override bool NegativeFlag
        {
            get { return false; }
        }

        public override bool CarryFlag
        {
            get { return false; }
        }
    }
}
