using System;
using System.Collections.Generic;
namespace Wren.Core.Replay
{
    public interface IReplayManager
    {
        void AddReplay(RecordedReplay replay);
        IEnumerable<RecordedReplay> GetGameReplays(Game game);
        void Save();
    }
}
