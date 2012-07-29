using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Persistence;

namespace Wren.Core.Statistics
{
    [ModuleDependency(typeof(PersistenceModule))]
    public class StatisticsModule : IModule
    {
        public const String StatisticsStatePersistenceProviderKey = "StatisticsStateProviderKey";
        public const String StatisticsPersistenceProviderKey = "StatisticsPersistenceProviderKey";

        public void Load(IModuleContext context)
        {
            var persistenceManager = context.ServiceLocator.GetInstance<IPersistenceManager>();
            persistenceManager.RegiserPersistenceProvider(StatisticsPersistenceProviderKey, () => new ApplicationDataXmlPersistenceProvider());
            persistenceManager.RegiserPersistenceProvider(StatisticsStatePersistenceProviderKey, () => new ApplicationDataXmlPersistenceProvider());

            context.ServiceLocator.RegisterSingleton<IStatisticsManager, StatisticsManager>();
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
