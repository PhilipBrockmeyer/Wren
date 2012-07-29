using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core
{
    public interface IEmulator
    {
        void Initialize(IEventAggregator eventAggregator);
        void GetSpecifications(out Int32 screenWidth, out Int32 screenHeight, out Int32 refreshRate, out PixelFormats pixelFormat);
        void SetRenderingSurface(IntPtr renderingSurface);
        void SetInput(InputState input);
        void LoadRom(Stream romData, Stream saveData);
        Boolean Run();
        Boolean IsRomValid(Stream romData);
        void DisableSound();
        void Quit();
    }
}
