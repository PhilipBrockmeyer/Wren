using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Wren.Core
{
    public class RenderingSourceFactory
    {

        public IRenderingSource Create(Int32 width, Int32 height, PixelFormats pixelFormat)
        {
            RenderingSurfaceInformation _renderingSurfaceInformation;        
            IntPtr memorySection;
            IntPtr renderingSurface;
            Int32 bitsPerPixel = 0;

            uint numPixels = (uint)(width * height);

            switch (pixelFormat)
            {
                case PixelFormats.Bgr101010:
                    break;
                case PixelFormats.Bgr24:
                    break;
                case PixelFormats.Bgr32:
                    break;
                case PixelFormats.Bgr555:
                    break;
                case PixelFormats.Bgr565:
                    bitsPerPixel = 16;
                    break;
                case PixelFormats.Bgra32:
                    break;
                case PixelFormats.BlackWhite:
                    break;
                case PixelFormats.Cmyk32:
                    break;
                case PixelFormats.Default:
                    break;
                case PixelFormats.Gray16:
                    break;
                case PixelFormats.Gray2:
                    break;
                case PixelFormats.Gray32Float:
                    break;
                case PixelFormats.Gray4:
                    break;
                case PixelFormats.Gray8:
                    break;
                case PixelFormats.Indexed1:
                    break;
                case PixelFormats.Indexed2:
                    break;
                case PixelFormats.Indexed4:
                    break;
                case PixelFormats.Indexed8:
                    break;
                case PixelFormats.Pbgra32:
                    break;
                case PixelFormats.Prgba128Float:
                    break;
                case PixelFormats.Prgba64:
                    break;
                case PixelFormats.Rgb128Float:
                    break;
                case PixelFormats.Rgb24:
                    break;
                case PixelFormats.Rgb48:
                    break;
                case PixelFormats.Rgba128Float:
                    break;
                case PixelFormats.Rgba64:
                    break;
                default:
                    break;
            }

            uint numBytes = numPixels * (uint)(bitsPerPixel / 8);

            memorySection = CreateFileMapping(INVALID_HANDLE_VALUE,
                                               IntPtr.Zero,
                                               PAGE_READWRITE,
                                               0,
                                               numBytes,
                                               null);
            

            renderingSurface =   MapViewOfFile(memorySection,
                                        FILE_MAP_ALL_ACCESS,
                                        0,
                                        0,
                                        numBytes);

            _renderingSurfaceInformation = new RenderingSurfaceInformation(width, height, bitsPerPixel, pixelFormat);

            return new RenderingSource(memorySection, renderingSurface, _renderingSurfaceInformation);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile,
                                               IntPtr lpFileMappingAttributes,
                                               uint flProtect,
                                               uint dwMaximumSizeHigh,
                                               uint dwMaximumSizeLow,
                                               string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
                                           uint dwDesiredAccess,
                                           uint dwFileOffsetHigh,
                                           uint dwFileOffsetLow,
                                           uint dwNumberOfBytesToMap);


        // Windows constants
        uint FILE_MAP_ALL_ACCESS = 0xF001F;
        uint PAGE_READWRITE = 0x04;
        IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    }
}
