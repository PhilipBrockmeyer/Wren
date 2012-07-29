using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class Symbol
    {
        public enum SymbolType
        {
            Byte,
            Word
        }

        public String Representation { get; set; }
        public SymbolType Type { get; set; }
        public Int32 Value { get; set; }
    }
}
