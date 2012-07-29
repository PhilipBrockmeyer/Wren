using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Events;
using Wren.Core;
using System.Windows.Interop;
using System.Windows.Controls;

namespace Wren.EventHandlers
{
    public class RenderingSurfaceCreatedEventHandler : Wren.Core.EventHandler<RenderingSurfaceCreatedEvent>
    {
        IEmulatorSurfaces _surfaces;
        Image _image;

        public RenderingSurfaceCreatedEventHandler(IEmulatorSurfaces surfaces, Image image)
        {
            _surfaces = surfaces;
            _image = image;
        }

        public override void Execute(RenderingSurfaceCreatedEvent e)
        {
            _surfaces.SetSurface(e.EmulationRunnerId,
                CreateBitmapSource(e.MemorySection, e.SurfaceInformation));

            _image.Source = _surfaces.GetSurface(e.EmulationRunnerId);
        }

        private InteropBitmap CreateBitmapSource(IntPtr surface, RenderingSurfaceInformation surfaceInformation)
        {
            int stride = (surfaceInformation.Width * surfaceInformation.BitsPerPixel + 7) / 8;

            return (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(surface,
                                                            surfaceInformation.Width,
                                                            surfaceInformation.Height,
                                                            System.Windows.Media.PixelFormats.Bgr565,
                                                            stride,
                                                            0);
        }

    }
}
