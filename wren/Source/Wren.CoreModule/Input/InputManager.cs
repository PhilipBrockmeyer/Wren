using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.DirectInput;

namespace Wren.Core.Input
{
    public sealed class InputManager
    {
        Keyboard _keyboard;
        IList<Joystick> _joysticks;

       
        public InputManager()
        {
            
        }

        private void InitializeDevices()
        {
            var handle = WrenCore.WindowHandle;

            _joysticks = new List<Joystick>();

            DirectInput di = new DirectInput();

            foreach (var device in di.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly))
            {
                if ((device.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
                {
                    Keyboard keyboard = new Keyboard(di);
                    keyboard.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    _keyboard = keyboard;
                }
                else if ((device.Type & DeviceType.Joystick) == DeviceType.Joystick)
                {
                    Joystick joystick = new Joystick(di, device.InstanceGuid);
                    joystick.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    _joysticks.Add(joystick);
                }
            }
        }

        public Keyboard GetKeyboard()
        {
            if (_keyboard == null)
                InitializeDevices();

            return _keyboard;
        }

        public Int32 GetJoystickCount()
        {
            if (_joysticks == null)
                InitializeDevices();

            return _joysticks.Count;
        }

        public Joystick GetJoystick(Int32 joystickIndex)
        {
            if (_joysticks == null)
                InitializeDevices();

            return _joysticks[joystickIndex];
        }
    }
}
