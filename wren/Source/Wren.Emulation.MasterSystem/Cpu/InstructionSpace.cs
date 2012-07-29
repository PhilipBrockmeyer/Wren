using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class InstructionSpace
    {
        public String Prefix { get; private set; }

        public InstructionSpace() : this(String.Empty) { }

        public InstructionSpace(String prefix)
        {
            Prefix = prefix;
        }
    }
}
