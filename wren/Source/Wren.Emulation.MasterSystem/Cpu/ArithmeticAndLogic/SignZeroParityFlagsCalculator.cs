using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.ArithmeticAndLogic
{
    public class SignZeroParityFlagsCalculator : FlagCalculator
    {
        Int32 _value;

        public SignZeroParityFlagsCalculator(Int32 value)
        {
            if (value > 0xFF || value < 0x00)
            {
                throw new ArgumentException("Value must be between 0x00 and 0xFF");
            }

            _value = value;
        }

        public override Boolean SignFlag
        {
            get { return (_value & 0x80) == 0x80; }
        }

        public override Boolean ZeroFlag
        {
            get { return _value == 0; }
        }

        public override Boolean Flag5
        {
            get { return (_value & Flags.Flag5) == Flags.Flag5; }
        }

        public override Boolean HalfCarryFlag
        {
            get { return false; }
        }

        public override Boolean Flag3
        {
            get { return (_value & Flags.Flag3) == Flags.Flag3; }
        }

        public override Boolean ParityOverflowFlag
        {
            get 
            {
                var parity = true;
                for (Int32 i = 0; i < 8; i++)
                {
                    if ((_value & (1 << i)) != 0)
                    {
                        parity = !parity;
                    }
                }

                return parity;
            }
        }

        public override Boolean NegativeFlag
        {
            get { return false; }
        }

        public override Boolean CarryFlag
        {
            get { return false; }
        }
    }
}
