using System;
using System.Collections.Generic;
namespace Wren.Core.GameLibrary
{
    public interface IGameLibraryManager
    {
        void UpdateGameLibrary(ICollection<GameInfo> games);
        void UpdateGameInfo(GameInfo game);

        void RegisterGameInfoProvider(IGameInfoProvider gameInfoProvider);
    }
}
