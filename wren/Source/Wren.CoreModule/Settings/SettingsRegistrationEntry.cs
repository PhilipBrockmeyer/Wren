using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Settings
{
    public class SettingsRegistrationEntry
    {
        public Type SettingsType { get; private set; }
        public Type TargetType { get; private set; }
        public SettingsScope Scope { get; private set; }

        public SettingsRegistrationEntry(Type settingsType, Type targetType, SettingsScope scope)
        {
            SettingsType = settingsType;
            TargetType = targetType;
            Scope = scope;
        }
    }
}
