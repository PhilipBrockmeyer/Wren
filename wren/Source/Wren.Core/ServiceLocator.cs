using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace Wren.Core
{
    public class ServiceLocator : IServiceLocator
    {
        public ServiceLocator()
        {
        }

        public T GetInstance<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }

        public Object GetInstance(Type type)
        {
            return ObjectFactory.GetInstance(type);
        }

        public T GetNamedInstance<T>(String name)
        {
            return ObjectFactory.GetNamedInstance<T>(name);
        }

        public void Register<TInterface, TImplementation>()
            where TImplementation : TInterface
        {
            ObjectFactory.Configure(c =>
                {
                    c.ForRequestedType<TInterface>().TheDefaultIsConcreteType<TImplementation>();
                });
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : TInterface
        {
            ObjectFactory.Configure(c =>
            {
                c.ForRequestedType<TInterface>()
                    .CacheBy(StructureMap.Attributes.InstanceScope.Singleton)
                    .TheDefaultIsConcreteType<TImplementation>();

            });
        }

        public void RegisterNamedInstance<TInterface, TImplementation>(String name)
            where TImplementation : TInterface
        {
            ObjectFactory.Configure(c =>
            {
                c.InstanceOf<TInterface>().Is.OfConcreteType<TImplementation>().WithName(name);
            });
        }
    }
}
