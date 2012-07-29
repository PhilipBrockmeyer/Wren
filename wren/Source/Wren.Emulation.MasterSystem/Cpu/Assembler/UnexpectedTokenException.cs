using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    [Serializable]
    public class UnexprectedTokenException : Exception
    {
        public UnexprectedTokenException() { }
        public UnexprectedTokenException(string message) : base(message) { }
        public UnexprectedTokenException(string message, Exception inner) : base(message, inner) { }
        protected UnexprectedTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
