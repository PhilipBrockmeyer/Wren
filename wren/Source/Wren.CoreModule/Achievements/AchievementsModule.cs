using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.GameLibrary;
using Wren.Core.Persistence;
using Wren.Core.Settings;
using Wren.Core.Services;

namespace Wren.Core.Achievements
{
    [ModuleDependency(typeof(GameLibraryModule))]
    [ModuleDependency(typeof(PersistenceModule))]
    [ModuleDependency(typeof(SettingsModule))]
    public class AchievementsModule : IModule
    {
        public const String AchievementsPersistenceProviderKey = "Achievements";
        // public const String SettingsPersistenceName = "Settings.config";

        public void Load(IModuleContext context)
        {
            var persistenceManager = context.ServiceLocator.GetInstance<IPersistenceManager>();
            persistenceManager.RegiserPersistenceProvider(AchievementsPersistenceProviderKey, () => new RavenDbPersistenceProvider());

            var settingsManager = context.ServiceLocator.GetInstance<ISettingsManager>();
            settingsManager.RegisterSettings<AchievementSettings>();

            AchievementSettings settings = new AchievementSettings();
            settings.DefinitionsLastSynchronized = new DateTime(2000, 1, 1);
            settings.StateLastDownloaded = new DateTime(2000, 1, 1);
            settingsManager.ApplySettings(settings);
           
            AchievementsManager manager = 
                new AchievementsManager(
                    new AchievementsService(), 
                    settingsManager, 
                    persistenceManager,
                    context.ServiceLocator.GetInstance<IEventAggregator>());

            var gameLibraryManager = context.ServiceLocator.GetInstance<IGameLibraryManager>();
            gameLibraryManager.RegisterGameInfoProvider(new AchievementGameInfoProvider(manager));
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
