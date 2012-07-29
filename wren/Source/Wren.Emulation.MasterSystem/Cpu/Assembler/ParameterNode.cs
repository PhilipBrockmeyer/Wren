using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class ParameterNode
    {
        public String Identifier { get; set; }
        public Int32 Number { get; set; }
        public Int32 NumberSize { get; set; }
        
        public override string ToString()
        {
            if (String.IsNullOrEmpty(Identifier))
                return "0x" + Number.ToString("X4");

            return Identifier;
        }
    }
}
