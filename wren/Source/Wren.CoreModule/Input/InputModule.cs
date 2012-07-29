using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Settings;

namespace Wren.Core.Input
{
    [ModuleDependency(typeof(SettingsModule))]
    public sealed class InputModule : IModule
    {
        public void Load(IModuleContext context)
        {
            var settingsManager = context.ServiceLocator.GetInstance<ISettingsManager>();
            settingsManager.RegisterSettings<KeyboardBindingSettings>();
            settingsManager.RegisterSettings<GamepadBindingSettings>();
            settingsManager.RegisterSettings<InputSettings>();

            SetDefaults(settingsManager);

            InputManager inputManager = new InputManager();

            context.InputSourceAssembler.ConfigureInputSource((c) =>
                {
                    var settings = settingsManager.LoadSettings<InputSettings>(c);

                    if (settings.IsUserInputEnabled)
                    {
                        var bindings = settingsManager.LoadSettings<KeyboardBindingSettings>(c);
                        KeyboardInputSource keyboard = new KeyboardInputSource(inputManager.GetKeyboard(), bindings);
                        return keyboard;
                    }

                    return null;

                });


            context.InputSourceAssembler.ConfigureInputSource((c) =>
                {
                    var settings = settingsManager.LoadSettings<InputSettings>(c);

                    if (settings.IsUserInputEnabled)
                    {
                        CompositeInputSource cis = new CompositeInputSource();

                        for (Int32 i = 0; i < inputManager.GetJoystickCount(); i++)
                        {
                            var bindings = settingsManager.LoadSettings<GamepadBindingSettings>(c);

                            if (i == 0)
                            {
                                GamepadInputSource gamepad = new GamepadInputSource(inputManager.GetJoystick(i), bindings.Gamepad1Bindings);
                                cis.AddInputSource(gamepad);
                            }
                            else if (i == 1)
                            {
                                GamepadInputSource gamepad = new GamepadInputSource(inputManager.GetJoystick(i), bindings.Gamepad2Bindings);
                                cis.AddInputSource(gamepad);
                            }
                        }

                        return cis;
                    }

                    return null;
                });
        }

        private void SetDefaults(ISettingsManager settingsManager)
        {
            var keybindings = new List<Wren.Core.Input.KeyboardBindingSettings.KeyBinding>();
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 0, Key = "UpArrow" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 1, Key = "DownArrow" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 2, Key = "LeftArrow" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 3, Key = "RightArrow" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 4, Key = "X" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 5, Key = "Z" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 6, Key = "C" });
            keybindings.Add(new KeyboardBindingSettings.KeyBinding() { ButtonId = 7, Key = "V" });
            var defaultKeybindings = new KeyboardBindingSettings() { Bindings = keybindings.ToArray() };
            settingsManager.ApplySettings(defaultKeybindings);

            var gamepadbindings1 = new List<Wren.Core.Input.GamepadBindingSettings.GamepadBinding>();
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 0, Button = -1 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 1, Button = -2 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 2, Button = -3 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 3, Button = -4 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 4, Button = 1 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 5, Button = 0 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 6, Button = 6 });
            gamepadbindings1.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 7, Button = 7 });

            var gamepadbindings2 = new List<Wren.Core.Input.GamepadBindingSettings.GamepadBinding>();
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 8, Button = -1 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 9, Button = -2 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 10, Button = -3 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 11, Button = -4 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 12, Button = 1 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 13, Button = 0 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 14, Button = 6 });
            gamepadbindings2.Add(new Wren.Core.Input.GamepadBindingSettings.GamepadBinding() { ButtonId = 15, Button = 7 });

            var defaultGamepadBindings = new GamepadBindingSettings() { Gamepad1Bindings = gamepadbindings1.ToArray(), Gamepad2Bindings = gamepadbindings2.ToArray() };
            settingsManager.ApplySettings(defaultGamepadBindings);
        }

        public void Unload(IModuleContext context)
        {
        }
    }
}
