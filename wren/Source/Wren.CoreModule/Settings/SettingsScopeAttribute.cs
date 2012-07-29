using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Settings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SettingsScopeAttribute : Attribute
    {
        public SettingsScope SettingsScope { get; private set; }

        public SettingsScopeAttribute(SettingsScope settingsScope)
        {
            SettingsScope = settingsScope;
        }
    }
}
