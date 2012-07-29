using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Input;
using Wren.Core.Settings;
using SlimDX.DirectInput;

namespace Wren.Core.Input
{
    [Serializable]
    [SettingsScope(SettingsScope.EmulatedSystem | SettingsScope.Game | SettingsScope.Global)]
    public sealed class KeyboardBindingSettings : ISettings
    {
        [Serializable]
        public struct KeyBinding
        {
            public Int32 ButtonId { get;  set; }
            public String Key { get; set; }
        }

        IList<KeyBinding> _bindingSettings { get; set; }

        public KeyBinding[] Bindings
        {
            get { return _bindingSettings.ToArray(); }
            set { _bindingSettings = new List<KeyBinding>(value); }
        }

        public KeyboardBindingSettings()
        {
            _bindingSettings = new List<KeyBinding>();
        }
    }
}
