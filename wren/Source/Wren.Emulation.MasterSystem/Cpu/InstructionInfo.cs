using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public class InstructionInfo
    {
        public Int32? Cycles { get; set; }
        public String Mnemonic { get; set; }
        public Int32 Opcode { get; set; }
        public String Prefix { get; set; }
        public String Postfix { get; set; }
        public Expression Body { get; set; }
        public Object InstructionObject { get; set; }
        public InstructionParameterMode ParameterMode { get; set; }

        public override string ToString()
        {
            return Mnemonic;
        }
    }
}
