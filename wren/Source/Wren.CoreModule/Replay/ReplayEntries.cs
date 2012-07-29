using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Replay
{
    [Serializable]
    public class ReplayEntries
    {
        IList<RecordedReplay> _recordedReplays;

        public RecordedReplay[] RecordedReplays
        {
            get { return _recordedReplays.ToArray(); }
            set { _recordedReplays = new List<RecordedReplay>(value); }
        }

        public ReplayEntries()
        {
            _recordedReplays = new List<RecordedReplay>();
        }

        public void AddReplay(RecordedReplay replay)
        {
            _recordedReplays.Add(replay);
        }
    }
}
