using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Wren
{
    public static class WrenWindows
    {
        public static Dispatcher Dispatcher { get; set; }
        public static MemoryFilterViewModel MemoryFilterViewModel { get; set; }
        public static MemoryFilter MemoryFilterWindow { get; set; }

    }
}
