using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class InvalidFlagCalculatorException : Exception
    {
        public InvalidFlagCalculatorException() { }
        public InvalidFlagCalculatorException(string message) : base(message) { }
        public InvalidFlagCalculatorException(string message, Exception inner) : base(message, inner) { }
        protected InvalidFlagCalculatorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
