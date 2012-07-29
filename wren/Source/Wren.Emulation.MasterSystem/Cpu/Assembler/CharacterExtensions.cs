using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public static class CharacterExtensions
    {
        public static Boolean IsNumeric(this char c)
        {
            if (new char[] {'0', '1', '2', '3', '4', '5', '6', '7','8', '9'}.Contains(c))
                return true;

            return false;
        }

        public static Boolean IsWhiteSpace(this char c)
        {
            if (c == ' ' || c == '\t')
                return true;

            return false;
        }
    }
}
