using System;
namespace Wren.Core.Settings
{
    public interface ISettingsEntryKeyBuilder
    {
        String BuildKey(Type settings);
        String BuildKey(Type settings,SettingsScope scope, EmulationContext context);
        String BuildKey(Type settings, Wren.Core.Game gameIdentifier);
        String BuildKey(Type settings, Wren.Core.EmulatedSystem emulatedSystemIdentifier);
    }
}
