using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Settings;

namespace Wren.Core.Directory
{
    [ModuleDependency(typeof(SettingsModule))]
    public class DirectoryModule : IModule
    {
        public void Load(IModuleContext context)
        {
            var settingsManager = context.ServiceLocator.GetInstance<ISettingsManager>();
            settingsManager.RegisterSettings<PathSettings>();

            context.ServiceLocator.RegisterSingleton<IDirectoryManager, DirectoryManager>();
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
