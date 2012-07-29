using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class InvalidInstructionDependencyException : Exception
    {
        public InvalidInstructionDependencyException() { }
        public InvalidInstructionDependencyException(string message) : base(message) { }
        public InvalidInstructionDependencyException(string message, Exception inner) : base(message, inner) { }
        protected InvalidInstructionDependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
