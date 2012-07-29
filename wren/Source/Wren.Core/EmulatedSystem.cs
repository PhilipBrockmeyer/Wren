using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class EmulatedSystem
    {
        public EmulatedSystem(String emulatedSystemName)
        {
            UniqueName = emulatedSystemName;
        }

        public String UniqueName { get; private set; }

        public override Int32 GetHashCode()
        {
            return UniqueName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is EmulatedSystem))
                return false;

            return UniqueName.Equals((obj as EmulatedSystem).UniqueName);
        }
    }
}
