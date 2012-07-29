using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;

namespace Wren.Core.Settings
{
    public class SettingsManager : ISettingsManager
    {
        IPersistenceManager _persistenceManager;
        IDictionary<Type, SettingsRegistrationEntry> _registeredSettings;
        SettingsEntries _entries;
        ISettingsEntryKeyBuilder _keyBuilder;

        public SettingsManager(IPersistenceManager persistenceManager, ISettingsEntryKeyBuilder keyBuilder)
        {
            _keyBuilder = keyBuilder;
            _persistenceManager = persistenceManager;
            _registeredSettings = new Dictionary<Type, SettingsRegistrationEntry>();
            _entries = new SettingsEntries();
        }

        public void ApplySettings(ISettings settings, EmulationContext context, SettingsScope scope)
        {
            String key = String.Empty;

            key = _keyBuilder.BuildKey(settings.GetType(), scope, context);

            var currentEntry = _entries.Entries.Where(e => e.Key == key).FirstOrDefault();

            if (currentEntry != null)
            {
                currentEntry.Settings = settings;
            }
            else
            {
                _entries.AddSettings(new SettingsEntry() { Key = key, Settings = settings });
            }
        }

        public void ApplySettings(ISettings settings, bool shouldOverrideCurrentSettings)
        {
            if (shouldOverrideCurrentSettings)
                ApplySettings(settings, EmulationContext.Empty, SettingsScope.Override);
            else
                ApplySettings(settings, EmulationContext.Empty, SettingsScope.Global);
        }

        public void ApplySettings(ISettings settings)
        {
            ApplySettings(settings, false);
        }

        public T LoadSettings<T>(EmulationContext context)
            where T : ISettings
        {
            if (!_registeredSettings.ContainsKey(typeof(T)))
                throw new ApplicationException(String.Format("No settings were registered for the type {0}", typeof(T)));

            var entry = _registeredSettings[typeof(T)];

            ISettings settings = GetSettingsEntry(context, entry);

            if (settings == null)
            {
                settings = (ISettings)Activator.CreateInstance(entry.SettingsType);
                var newEntry = new SettingsEntry() { Settings = settings, Key = _keyBuilder.BuildKey(entry.SettingsType, entry.Scope, context) };
                _entries.AddSettings(newEntry);
                return (T)settings;
            }

            return (T)settings;
        }
        
        private ISettings GetSettingsEntry(EmulationContext context, SettingsRegistrationEntry entry)
        {
            if ((entry.Scope & SettingsScope.Override) == SettingsScope.Override)
            {
                String key = _keyBuilder.BuildKey(entry.SettingsType, SettingsScope.Override, context);
                var settingEntry = _entries.Entries.Where(e => e.Key == key).FirstOrDefault();

                if (settingEntry != null)
                    return settingEntry.Settings;
            }

            if ((entry.Scope & SettingsScope.Game) == SettingsScope.Game)
            {
                String key = _keyBuilder.BuildKey(entry.SettingsType, context.Game);
                var settingEntry = _entries.Entries.Where(e => e.Key == key).FirstOrDefault();

                if (settingEntry != null)
                    return settingEntry.Settings;
            }

            if ((entry.Scope & SettingsScope.EmulatedSystem) == SettingsScope.EmulatedSystem)
            {
                String key = _keyBuilder.BuildKey(entry.SettingsType, context.EmulatedSystem);
                var settingEntry = _entries.Entries.Where(e => e.Key == key).FirstOrDefault();

                if (settingEntry != null)
                    return settingEntry.Settings;
            }

            if ((entry.Scope & SettingsScope.Global) == SettingsScope.Global)
            {
                String key = _keyBuilder.BuildKey(entry.SettingsType);
                var settingEntry = _entries.Entries.Where(e => e.Key == key).FirstOrDefault();

                if (settingEntry != null)
                    return settingEntry.Settings;
            }

            return null;
        }

        public void Load()
        {
            var loadedEntries = _persistenceManager.Load<SettingsEntries>(SettingsModule.SettingsPersistenceName, SettingsModule.SettingsPersistenceProviderKey);

            if (loadedEntries == null)
                return;

            foreach (var entry in loadedEntries.Entries)
            {
                var currentEntry = _entries.Entries.Where(e => e.Key == entry.Key).FirstOrDefault();

                if (currentEntry == null)
                    _entries.AddSettings(new SettingsEntry() { Key = entry.Key, Settings = entry.Settings });
                else
                    currentEntry.Settings = entry.Settings;
            }
        }

        public void SaveSettings()
        {
            _persistenceManager.Save(SettingsModule.SettingsPersistenceName, SettingsModule.SettingsPersistenceProviderKey, _entries);
        }

        public void RegisterSettings<TSettings>()
            where TSettings : ISettings
        {
            if (typeof(TSettings).GetCustomAttributes(typeof(SerializableAttribute), false).Count() == 0)
                throw new ApplicationException(String.Format("The settings type {0} must be serializable.", typeof(TSettings)));

            var attr = typeof(TSettings).GetCustomAttributes(typeof(SettingsScopeAttribute), false).FirstOrDefault() as SettingsScopeAttribute;

            if (attr == null)
                throw new ApplicationException(String.Format("The type {0} must be decorated with the SettingsScope Attribute.", typeof(TSettings)));

            if (_registeredSettings.ContainsKey(typeof(TSettings)))
                throw new ApplicationException(String.Format("The type {0} has already has settings registered with it.", typeof(TSettings)));

            SettingsRegistrationEntry entry = new SettingsRegistrationEntry(typeof(TSettings), typeof(TSettings), attr.SettingsScope);
            _registeredSettings.Add(typeof(TSettings), entry);
        }
    }
}
