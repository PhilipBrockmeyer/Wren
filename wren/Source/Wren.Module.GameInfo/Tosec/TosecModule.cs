using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.GameLibrary;

namespace Wren.GameInfoProviders.Tosec
{
    [ModuleDependency(typeof(GameLibraryModule))]
    public class TosecModule : IModule
    {
        public void Load(IModuleContext context)
        {
            var gameLibraryManager = context.ServiceLocator.GetInstance<IGameLibraryManager>();
            gameLibraryManager.RegisterGameInfoProvider(new TosecGameInfoProvider() );
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
