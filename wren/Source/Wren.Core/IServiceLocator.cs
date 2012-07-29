using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IServiceLocator
    {
        T GetInstance<T>();
        void Register<TInterface, TImplementation>()
            where TImplementation : TInterface;
        void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : TInterface;
    }
}
