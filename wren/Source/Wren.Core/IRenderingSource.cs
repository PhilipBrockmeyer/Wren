using System;
namespace Wren.Core
{
    public interface IRenderingSource
    {
        IntPtr RenderingSurface { get; }
        IntPtr MemorySection { get; }
        RenderingSurfaceInformation SurfaceInformation { get; }
    }
}
