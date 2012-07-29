using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class LibraryNotRegisteredException : Exception
    {
        public LibraryNotRegisteredException() { }
        public LibraryNotRegisteredException(string message) : base(message) { }
        public LibraryNotRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected LibraryNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
