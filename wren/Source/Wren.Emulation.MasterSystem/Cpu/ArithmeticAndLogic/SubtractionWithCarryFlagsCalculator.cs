using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.ArithmeticAndLogic
{
    public class SubtractionWithCarryFlagsCalculator : SubtractionFlagsCalculator
    {
        public SubtractionWithCarryFlagsCalculator(Int32 value1, Int32 value2)
            : base(value1, value2 + 1, false)
	    {
            if (value1 > 0xFF || value2 > 0xFF || value1 < 0x00 || value2 < 0x00)
            {
                throw new ArgumentException("Values must be between 0x00 and 0xFF");
            }
	    }

        public override Boolean HalfCarryFlag
        {
            get
            {
                return ((Value1 - Value2) & 0x0F) >= (Value1 & 0x0F);
            }
        }

        public override Boolean ParityOverflowFlag
        {
            get
            {
                // If the signs are equal we cannot have overflow.
                if ((Value1 & 0x80) == ((Value2 - 1) & 0x80))
                    return false;

                // If the result of the subtraction changes the sign, overflow has occured (given that the signs are opposite).
                var initialSign = Value1 & 0x80;
                if (((Value1 - Value2) & 0x80) != initialSign)
                    return true;

                return false;
            }
        }

        public override Boolean CarryFlag
        {
            get
            {
                return ((Value1 - Value2) & 0xFF) >= Value1;
            }
        }
    }
}
