using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IModule
    {
        void Load(IModuleContext context);
        void Unload(IModuleContext context);
    }
}
