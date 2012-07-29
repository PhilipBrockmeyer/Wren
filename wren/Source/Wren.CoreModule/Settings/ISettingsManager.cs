using System;

namespace Wren.Core.Settings
{
    public interface ISettingsManager
    {
        void ApplySettings(ISettings settings, EmulationContext context, SettingsScope scope);
        void ApplySettings(ISettings settings, Boolean shouldOverrideCurrentSettings);
        void ApplySettings(ISettings settings);

        T LoadSettings<T>(EmulationContext context)
            where T : ISettings;

        void Load();

        void RegisterSettings<TSettings>() 
            where TSettings : ISettings;

        void SaveSettings();
    }
}
