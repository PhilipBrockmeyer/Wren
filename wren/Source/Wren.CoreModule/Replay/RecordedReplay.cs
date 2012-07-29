using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Replay
{
    [Serializable]
    public class RecordedReplay
    {
        public String GameId { get; set; }
        public String FileName { get; set; }
        public DateTime RecordedDate { get; set; }

        public RecordedReplay(String gameId, String fileName, DateTime recordedDate)
        {
            GameId = gameId;
            FileName = fileName;
            RecordedDate = recordedDate;
        }

        public RecordedReplay()
        {

        }
    }
}
