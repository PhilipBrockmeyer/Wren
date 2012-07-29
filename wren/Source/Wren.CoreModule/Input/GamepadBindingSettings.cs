using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.Core.Input
{
    [Serializable]
    [SettingsScope(SettingsScope.EmulatedSystem | SettingsScope.Game | SettingsScope.Global)]
	public class GamepadBindingSettings : ISettings
    {
        [Serializable]
        public struct GamepadBinding
        {
            public Int32 ButtonId { get;  set; }
            public Int32 Button { get; set; }

            public String GetButtonName()
            {
                switch (Button)
                {
                    case -1:
                        return "Gamepad {0} Up";

                    case -2:
                        return "Gamepad {0} Down";
                    
                    case -3:
                        return "Gamepad {0} Left";

                    case -4:
                        return "Gamepad {0} Right";

                    case 0:
                        return "Gamepad {0} A";

                    case 1:
                        return "Gamepad {0} B";
                    
                    case 2:
                        return "Gamepad {0} X";
                    
                    case 3:
                        return "Gamepad {0} Y";
                }

                return String.Empty;
            }

            public static Int32 ButtonFromString(String buttonName)
            {
                switch (buttonName)
                {
                    case "Up":
                        return -1;

                    case "Down":
                        return -2;

                    case "Left":
                        return -3;

                    case "Right":
                        return -4;
                    
                    case "A":
                        return 0;
                    
                    case "B":
                        return 1;
                    
                    case "X":
                        return 2;

                    case "Y":
                        return 3;
                }

                return 0;
            }
        }

        IList<GamepadBinding> _bindingSettings1 { get; set; }
        IList<GamepadBinding> _bindingSettings2 { get; set; }

        public GamepadBinding[] Gamepad1Bindings
        {
            get { return _bindingSettings1.ToArray(); }
            set { _bindingSettings1 = new List<GamepadBinding>(value); }
        }

        public GamepadBinding[] Gamepad2Bindings
        {
            get { return _bindingSettings2.ToArray(); }
            set { _bindingSettings2 = new List<GamepadBinding>(value); }
        }

        public GamepadBindingSettings()
        {
            _bindingSettings1 = new List<GamepadBinding>();
            _bindingSettings2 = new List<GamepadBinding>();
        }
	}
}
