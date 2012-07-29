using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class UnknownInstructionException : Exception
    {
        public UnknownInstructionException() { }
        public UnknownInstructionException(string message) : base(message) { }
        public UnknownInstructionException(string message, Exception inner) : base(message, inner) { }
        protected UnknownInstructionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public UnknownInstructionException(Int32 address) : this(String.Format("Instruction at {0:X4} could not be resolved.", address))
        {

        }
    }
}
