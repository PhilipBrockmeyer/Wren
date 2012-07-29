using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Data.Entities
{
    public class AchievementDefinition : Entity
    {
        public virtual String RomMd5 { get; set; }
        public virtual String Description { get; set; }
        public virtual String UnlockedImageUrl { get; set; }
        public virtual String LockedImageUrl { get; set; }
        public virtual String Code { get; set; }
        public virtual String ShortDisplayFormat { get; set; }
        public virtual String LongDisplayFormat { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual String Title { get; set; }
    }
}
