using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Wren.Core
{
    public class RenderingSource : IRenderingSource
    {
        public IntPtr MemorySection { get; private set; }
        public IntPtr RenderingSurface { get; private set; }
        public RenderingSurfaceInformation SurfaceInformation { get; private set; }

        public RenderingSource(IntPtr memorySection, IntPtr renderingSurface, RenderingSurfaceInformation surfaceInformation)
        {
            MemorySection = memorySection;
            RenderingSurface = renderingSurface;
            SurfaceInformation = surfaceInformation;
        }
    }
}
