using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core
{
    public interface IStatefulEmulator
    {
        void SaveState(BinaryWriter writer);
        void LoadState(BinaryReader reader);
    }
}
