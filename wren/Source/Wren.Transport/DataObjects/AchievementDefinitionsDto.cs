using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Transport.DataObjects
{
    public class AchievementDefinitionDto
    {
        public String Id { get; set; }
        public String RomMd5 { get; set; }
        public String Description { get; set; }
        public String Title { get; set; }
        public String UnlockedImageUrl { get; set; }
        public String LockedImageUrl { get; set; }
        public String Code { get; set; }
        public String ShortDisplayFormat { get; set; }
        public String LongDisplayFormat { get; set; }
    }

    public class AchievementDefinitionsDto
    {       
        public AchievementDefinitionDto[] Definitions { get; set; }
        public DateTime UpdateTimestamp { get; set; }
    }
}
