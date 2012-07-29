using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.ViewModels
{
    [Serializable]
    [SettingsScope(SettingsScope.Global)]
    public class LogOnSettings : ISettings
    {
        public String UserId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public LogOnSettings()
        {
        }
    }
}
