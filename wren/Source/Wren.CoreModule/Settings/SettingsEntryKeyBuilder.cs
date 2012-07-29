using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Settings
{
    public class SettingsEntryKeyBuilder : ISettingsEntryKeyBuilder
    {
        public String BuildKey(Type settings, Game gameIdentifier)
        {
            return settings.Name + " " + gameIdentifier.Id;
        }

        public String BuildKey(Type settings, EmulatedSystem emulatedSystemIdentifier)
        {
            return settings.Name + " " + emulatedSystemIdentifier.UniqueName;
        }

        public String BuildKey(Type settings)
        {
            return settings.Name + " Global";
        }


        public String BuildKey(Type settings, SettingsScope scope, EmulationContext context)
        {
            if (scope == SettingsScope.None)
                throw new ApplicationException(String.Format("Could not create settings key for type {0}.  Settings scope is set to 'None'", settings));

            if ((scope & SettingsScope.Global) == SettingsScope.Global)
                return BuildKey(settings);

            if ((scope & SettingsScope.EmulatedSystem) == SettingsScope.EmulatedSystem)
                return BuildKey(settings, context.EmulatedSystem);

            if ((scope & SettingsScope.Game) == SettingsScope.Game)
                return BuildKey(settings, context.Game);

            if ((scope & SettingsScope.Override) == SettingsScope.Override)
                return "OVERRIDE " + settings.Name;

            throw new ApplicationException(String.Format("There was an error creating the settings key for type {0]", settings));
        }
    }
}
