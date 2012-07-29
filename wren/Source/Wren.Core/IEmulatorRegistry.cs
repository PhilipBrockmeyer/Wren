using System;
namespace Wren.Core
{
    public interface IEmulatorRegistry
    {
        IEmulator GetEmulator(EmulatedSystem emulatedSystem, IntPtr handle);
        void RegisterEmulator<TEmulator>(EmulatedSystem emulatedSystem) where TEmulator : IEmulator;
    }
}
