using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ModuleDependencyAttribute : Attribute
    {
        public Type Module { get; private set; }

        public ModuleDependencyAttribute(Type module)
        {
            Module = module;
        }
    }
}
