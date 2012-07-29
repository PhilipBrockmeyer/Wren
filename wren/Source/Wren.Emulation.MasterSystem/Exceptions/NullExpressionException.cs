using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class NullExpressionException : Exception
    {
        public NullExpressionException() { }
        public NullExpressionException(string message) : base(message) { }
        public NullExpressionException(string message, Exception inner) : base(message, inner) { }
        protected NullExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
