using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.ArithmeticAndLogic
{
    public class SubtractionFlagsCalculator
        : FlagCalculator
    {
        protected Int32 Value1 { get; private set; }
        protected Int32 Value2 { get; private set; }

        public SubtractionFlagsCalculator(Int32 value1, Int32 value2)
            : this(value1, value2, true) { }

        internal SubtractionFlagsCalculator(Int32 value1, Int32 value2, Boolean shouldThrow)
        {
            if (shouldThrow && (value1 > 0xFF || value2 > 0xFF || value1 < 0x00 || value2 < 0x00))
            {
                throw new ArgumentException("Values must be between 0x00 and 0xFF");
            }

            Value1 = value1;
            Value2 = value2;
        }

        public override Boolean SignFlag
        {
            get
            {
                return ((Value1 - Value2) & 0x80) != 0;
            }
        }

        public override Boolean ZeroFlag
        {
            get
            {
                Int32 result = (Value1 - Value2) & 0xFF;

                if (result == 0)
                    return true;

                return false;
            }
        }

        public override Boolean Flag5
        {
            get
            {
                return ((Value1 - Value2) & 0x20) != 0;
            }
        }

        public override Boolean HalfCarryFlag
        {
            get
            {
                return ((Value1 - Value2) & 0x0F) > (Value1 & 0x0F);
            }
        }

        public override Boolean Flag3
        {
            get
            {
                return ((Value1 - Value2) & 0x08) != 0;
            }
        }

        public override Boolean ParityOverflowFlag
        {
            get
            {
                // If the signs are equal we cannot have overflow.
                if ((Value1 & 0x80) == (Value2 & 0x80))
                    return false;

                // If the result of the subtraction changes the sign, overflow has occured (given that the signs are opposite).
                var initialSign = Value1 & 0x80;
                if (((Value1 - Value2) & 0x80) != initialSign)
                    return true;

                return false;
            }
        }

        public override Boolean NegativeFlag
        {
            get
            {
                return true;
            }
        }

        public override Boolean CarryFlag
        {
            get
            {
                return ((Value1 - Value2) & 0xFF) > Value1;
            }
        }
    }
}
