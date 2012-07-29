using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Settings;
using Wren.Core.Input;

namespace Wren.Module.Core.UI.Windows.WrenUI
{
    [ModuleDependency(typeof(SettingsModule))]
    [ModuleDependency(typeof(InputModule))]
    public class WrenUIModule : IModule
    {
        public void Load(IModuleContext context)
        {
            var settingsManager = context.ServiceLocator.GetInstance<ISettingsManager>();

            var keybindings = new List<Wren.Core.Input.KeyboardBindingSettings.KeyBinding>();
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 100, Key = "Escape" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 101, Key = "M" });
            var defaultKeybindings = new KeyboardBindingSettings() { Bindings = keybindings.ToArray() };
            settingsManager.ApplySettings(defaultKeybindings, new EmulationContext(Game.Empty, new EmulatedSystem("WrenGame")), SettingsScope.EmulatedSystem);
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
