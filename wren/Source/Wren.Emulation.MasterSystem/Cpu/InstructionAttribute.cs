using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class InstructionAttribute : Attribute
    {
        // Required Properies
        public Int32 Opcode { get; private set; }
        public String Mnemonic { get; private set; }
        
        // Positional Properties
        public String Prefix { get; set; }
        public String Postfix { get; set; }
        public Int32 Cycles { get; set; }
        public InstructionParameterMode ParameterMode { get; set; }
        
        public InstructionAttribute(String mnemonic, Int32 opcode)
        {
            Opcode = opcode;
            Mnemonic = mnemonic;
        }
    }
}
