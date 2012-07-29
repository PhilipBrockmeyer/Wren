using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class RenderingSurfaceCreatedEvent : IEvent
    {
        public Guid EmulationRunnerId { get; private set; }
        public IntPtr MemorySection { get; set; }
        public IntPtr Surface { get; private set; }
        public RenderingSurfaceInformation SurfaceInformation { get; private set; }

        public RenderingSurfaceCreatedEvent(Guid emulationRunnerId, IntPtr memorySection, IntPtr surface, RenderingSurfaceInformation surfaceInformation)
        {
            EmulationRunnerId = emulationRunnerId;
            MemorySection = memorySection;
            Surface = surface;
            SurfaceInformation = surfaceInformation;
        }
    }
}
