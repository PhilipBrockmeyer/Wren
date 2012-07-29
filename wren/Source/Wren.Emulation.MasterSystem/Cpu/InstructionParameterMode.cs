using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public enum InstructionParameterMode
    {
        None,
        Byte,
        Word,
        Index,
        Address,
        IndexAndByte
    }
}
