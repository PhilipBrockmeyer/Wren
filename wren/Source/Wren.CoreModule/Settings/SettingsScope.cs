using System;
namespace Wren.Core.Settings
{
    [Flags]
    public enum SettingsScope
    {
        None = 0x00,
        Global = 0x01,
        EmulatedSystem = 0x02,
        Game = 0x04,
        Override = 0x08
    }
}