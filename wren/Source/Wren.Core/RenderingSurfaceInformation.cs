using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class RenderingSurfaceInformation
    {
        public Int32 Width { get; private set; }
        public Int32 Height { get; set; }
        public Int32 BitsPerPixel { get; set; }
        public PixelFormats PixelFormat { get; private set; }

        public RenderingSurfaceInformation(Int32 width, Int32 height, Int32 bitsPerPixel, PixelFormats pixelFormat)
        {
            Width = width;
            Height = height;
            BitsPerPixel = bitsPerPixel;
            PixelFormat = pixelFormat;
        }
    }
}
