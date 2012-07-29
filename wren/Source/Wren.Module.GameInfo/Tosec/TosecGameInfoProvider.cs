using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.GameLibrary;
using Wren.Module.GameInfoProviders.Tosec;
using Wren.Core;

namespace Wren.GameInfoProviders.Tosec
{
    public class TosecGameInfoProvider : IGameInfoProvider
    {
        TosecService service;

        public TosecGameInfoProvider()
        {
            service = new TosecService();
        }

        public void UpdateGameInfo(GameInfo gameInfo)
        {
            var md5 = gameInfo.Game.Id;
            var info = service.GetGameInfo(md5);

            if (info == null)
                return;

            gameInfo.SetValue("Name", info.Name);
            gameInfo.SetValue("Country", info.Country);
            gameInfo.SetValue("Publisher", info.Publisher);
            gameInfo.SetValue("Year", info.Year);
            gameInfo.SetValue("System", info.System);
        }

        public GameInfoProviderUpdateMode UpdateMode
        {
            get { return GameInfoProviderUpdateMode.InitializeOnly; }
        }
    }
}
