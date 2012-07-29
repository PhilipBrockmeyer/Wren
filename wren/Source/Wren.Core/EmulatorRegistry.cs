using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class EmulatorRegistry : IEmulatorRegistry
    {
        ServiceLocator _serviceLocator;

        public EmulatorRegistry(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IEmulator GetEmulator(EmulatedSystem emulatedSystem, IntPtr handle)
        {
            return _serviceLocator.GetNamedInstance<IEmulator>(emulatedSystem.UniqueName);
        }

        public void RegisterEmulator<TEmulator>(EmulatedSystem emulatedSystem) 
            where TEmulator : IEmulator
        {
            _serviceLocator.RegisterNamedInstance<IEmulator, TEmulator>(emulatedSystem.UniqueName);
        }
    }
}
