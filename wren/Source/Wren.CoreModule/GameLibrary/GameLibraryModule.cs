using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Directory;
using Wren.Core.Settings;
using Wren.Core.Persistence;

namespace Wren.Core.GameLibrary
{
    [ModuleDependency(typeof(DirectoryModule))]
    [ModuleDependency(typeof(PersistenceModule))]
    public class GameLibraryModule : IModule
    {
        public const String RomExtensionKey = "RomExtensionKey";
        public const String RomPathKey = "RomFilePath";
        public const String GameLibraryPersistenceProviderKey = "GameLibrary";
        public const String GameLibraryPersistenceName = "GameLibrary.xml";

        public void Load(IModuleContext context)
        {
            var directoryManager = context.ServiceLocator.GetInstance<IDirectoryManager>();
            directoryManager.RegisterExtensionKey(RomExtensionKey, "nes");
            directoryManager.RegisterExtensionKey(RomExtensionKey, "bin");
            directoryManager.RegisterExtensionKey(RomExtensionKey, "sms");
            // directoryManager.RegisterExtensionKey(RomExtensionKey, "zip");
            
            var persistenceManager = context.ServiceLocator.GetInstance<IPersistenceManager>();
            persistenceManager.RegiserPersistenceProvider(GameLibraryPersistenceProviderKey, () => new ApplicationDataXmlPersistenceProvider());

            context.ServiceLocator.RegisterSingleton<IGameLibraryManager, GameLibraryManager>();
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
