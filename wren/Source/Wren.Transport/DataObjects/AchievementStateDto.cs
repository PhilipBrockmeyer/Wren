using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Transport.DataObjects
{
    public class AchievementStateDto
    {
        public String Id { get; set; }
        public String AchievementDefinitionId { get; set; }
        public String RomMd5 { get; set; }
        public String UserId { get; set; }
        public Boolean IsUnlocked { get; set; }
        public String State { get; set; }
        public DateTime ServerDate { get; set; }
        public Int32 Version { get; set; }
    }

    public class AchievementStateDownloadDto
    {
        public AchievementStateDto[] State { get; set; }
        public DateTime UpdateTimestamp { get; set; }
    }
}
