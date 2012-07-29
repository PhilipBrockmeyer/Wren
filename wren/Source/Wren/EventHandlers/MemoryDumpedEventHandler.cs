using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Debugging;
using Wren.Core;

namespace Wren.EventHandlers
{
    public class MemoryDumpedEventHandler : Wren.Core.EventHandler<MemoryDumpedEvent>
    {
        public override void Execute(MemoryDumpedEvent e)
        {
            WrenWindows.Dispatcher.BeginInvoke((Action)(() =>
            {
                WrenWindows.MemoryFilterWindow.Show();
                WrenWindows.MemoryFilterViewModel.SetMemory(e.MemoryDump, e.BaseAddress);
            }));
        }
    }
}
