using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Data.Entities
{
    public class AchievementState : Entity
    {
        public virtual String Id { get; set; }
        public virtual String AchievementDefinitionId { get; set; }
        public virtual String UserId { get; set; }
        public virtual String RomMd5 { get; set; }
        public virtual Boolean IsUnlocked { get; set; }
        public virtual String State { get; set; }
        public virtual DateTime ServerDate { get; set; }
        public virtual Int32 Version { get; set; }
    }
}
