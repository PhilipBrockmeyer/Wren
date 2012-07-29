using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;

namespace Wren.Core.Replay
{
    public class ReplayManager : IReplayManager
    {
        IPersistenceManager _persistenceManager;
        ReplayEntries _entries;

        public ReplayManager(IPersistenceManager persistenceManager)
        {
            _persistenceManager = persistenceManager;

            _entries = _persistenceManager.Load<ReplayEntries>(ReplayModule.ReplayManagerLibraryPersistenceName, ReplayModule.ReplayManagerPersistenceProviderKey);

            if (_entries == null)
                _entries = new ReplayEntries();
        }

        public IEnumerable<RecordedReplay> GetGameReplays(Game game)
        {
            return _entries.RecordedReplays.Where(r => r.GameId == game.Id)
                .OrderByDescending(r => r.RecordedDate);
        }

        public void AddReplay(RecordedReplay replay)
        {
            _entries.AddReplay(replay);
        }

        public void Save()
        {
            _persistenceManager.Save(ReplayModule.ReplayManagerLibraryPersistenceName, ReplayModule.ReplayManagerPersistenceProviderKey, _entries);
        }
    }
}
