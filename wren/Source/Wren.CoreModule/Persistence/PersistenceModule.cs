using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using System.IO;

namespace Wren.Core.Persistence
{
    public sealed class PersistenceModule : IModule
    {
        public void Load(IModuleContext context)
        {
            context.ServiceLocator.RegisterSingleton<IPersistenceManager, PersistenceManager>();
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
