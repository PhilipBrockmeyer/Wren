using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.Core.Achievements
{
    [Serializable]
    [SettingsScope(SettingsScope.Global)]
    public class AchievementSettings : ISettings
    {
        public DateTime DefinitionsLastSynchronized { get; set; }
        public DateTime StateLastDownloaded { get; set; }
    }
}


