using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.GameLibrary
{
    public interface IGameInfoProvider
    {
        GameInfoProviderUpdateMode UpdateMode { get; }
        void UpdateGameInfo(GameInfo gameInfo);
    }
}
