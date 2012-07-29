using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class ReadOnlyException : Exception
    {
        public ReadOnlyException() { }
        public ReadOnlyException(string message) : base(message) { }
        public ReadOnlyException(string message, Exception inner) : base(message, inner) { }
        protected ReadOnlyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public ReadOnlyException(Int32 address, Byte value) : this(String.Format("An attempt was made to write to the read only address : 0x{0:X4}.  Value written : 0x{1:X2}", address, value)) { }
    }
}
