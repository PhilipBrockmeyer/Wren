using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace Wren.Core
{
    public static class WrenCore
    {
        static ServiceLocator _serviceLocator;
        static ModuleLoader _loader;
        static IModuleContext _context;

        public static IntPtr WindowHandle { get; set;  }
        public static String UserId { get; set; }
        public static Boolean IsOffline { get; set; }

        public static String ServerAddress { get; private set; }

        public static void Initialize()
        {
            ServerAddress = "localhost:11648";

            ObjectFactory.Configure(c =>
            {
                c.ForRequestedType<IFrameRateTimerFactory>().TheDefaultIsConcreteType<FrameRateTimerFactory>().CacheBy(StructureMap.Attributes.InstanceScope.Singleton);
                c.ForRequestedType<IEmulationRunner>().TheDefault.Is.OfConcreteType<EmulationRunner>();
                c.ForRequestedType<IEventAggregator>().TheDefaultIsConcreteType<EventAggregator>().CacheBy(StructureMap.Attributes.InstanceScope.Singleton);
            });

            _serviceLocator = new ServiceLocator();

            _serviceLocator.RegisterSingleton<IInputSourceAssembler, InputSourceAssembler>();
            _serviceLocator.RegisterSingleton<IEmulatorRegistry, EmulatorRegistry>();

            _loader = new ModuleLoader();
            _context = new ModuleContext()
                {
                    ServiceLocator = _serviceLocator,
                    InputSourceAssembler = _serviceLocator.GetInstance<IInputSourceAssembler>(),
                    EmulatorRegistry = _serviceLocator.GetInstance<IEmulatorRegistry>()
                };
            _loader.LoadModules(_context);        
        }

        public static ServiceLocator GetServiceLocator()
        {
            return _serviceLocator;
        }

        public static void Unload()
        {
            _loader.UnloadModules(_context);
        }
    }
}
