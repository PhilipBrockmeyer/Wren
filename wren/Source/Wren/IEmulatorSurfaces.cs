using System;
namespace Wren
{
    public interface IEmulatorSurfaces
    {
        System.Windows.Interop.InteropBitmap GetSurface(Guid emulationRunnerId);
        void SetSurface(Guid emulationRunnerId, System.Windows.Interop.InteropBitmap bitmap);
    }
}
