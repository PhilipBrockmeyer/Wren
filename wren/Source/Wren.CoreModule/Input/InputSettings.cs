using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.Core.Input
{
    [Serializable]
    [SettingsScope(SettingsScope.Override)]
    public class InputSettings : ISettings
    {
        public Boolean IsUserInputEnabled { get; set; }

        public InputSettings()
        {
            IsUserInputEnabled = true;
        }
    }
}
