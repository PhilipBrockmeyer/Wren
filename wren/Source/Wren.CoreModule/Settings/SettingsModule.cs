using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;

namespace Wren.Core.Settings
{
    [ModuleDependency(typeof(PersistenceModule))]
    public class SettingsModule : IModule
    {
        public const String SettingsPersistenceProviderKey = "Settings";
        public const String SettingsPersistenceName = "Settings.config";

        public SettingsModule()
        {
        }

        public void Load(IModuleContext context)
        {
            var persistenceManager = context.ServiceLocator.GetInstance<IPersistenceManager>();
            persistenceManager.RegiserPersistenceProvider(SettingsPersistenceProviderKey, () => new ApplicationDataXmlPersistenceProvider());

            context.ServiceLocator.RegisterSingleton<ISettingsManager, SettingsManager>();
            context.ServiceLocator.Register<ISettingsEntryKeyBuilder, SettingsEntryKeyBuilder>();
        }

        public void Unload(IModuleContext context)
        {
            context.ServiceLocator.GetInstance<ISettingsManager>().SaveSettings();
        }
    }
}
