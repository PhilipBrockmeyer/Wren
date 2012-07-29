using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Events;
using Wren.Core;
using System.Windows.Threading;

namespace Wren.EventHandlers
{
    public class FrameRenderedEventHandler : Wren.Core.EventHandler<FrameRenderedEvent>
    {
        IEmulatorSurfaces _surfaces;
        Dispatcher _dispatcher;

        public FrameRenderedEventHandler(IEmulatorSurfaces surfaces, Dispatcher dispatcher)
        {
            _surfaces = surfaces;
            _dispatcher = dispatcher;
        }

        public override void Execute(FrameRenderedEvent e)
        {
            var surface = _surfaces.GetSurface(e.EmulationRunnerId);
            _dispatcher.BeginInvoke((Action)(() => surface.Invalidate()));
        }
    }
}
