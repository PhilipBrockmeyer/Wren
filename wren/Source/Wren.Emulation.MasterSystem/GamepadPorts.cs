using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;

namespace Wren.Emulation.MasterSystem
{
    public class GamepadPorts : IPortAddressableSystemComponent
    {
        InputState _state;

        public GamepadPorts()
        {

        }

        public void RegisterPortAddresses(IPortManager portManager)
        {
            portManager.RegisterWritePort(0x3F, WriteNationalisation);
            portManager.RegisterPort(0xDC, WriteGamepadPort1, ReadGamepadPort1);
            portManager.RegisterPort(0xDD, WriteGamepadPort2, ReadGamepadPort2);
        }

        public void SetInputState(InputState state)
        {
            _state = state;
        }

        public Byte ReadGamepadPort1()
        {
            return (Byte)(
                   (_state.GetIsButtonPresed(0) ? 0x00 : 0x01) // P1 Up
                 | (_state.GetIsButtonPresed(1) ? 0x00 : 0x02) // P1 Down
                 | (_state.GetIsButtonPresed(2) ? 0x00 : 0x04) // P1 Left
                 | (_state.GetIsButtonPresed(3) ? 0x00 : 0x08) // P1 Right
                 | (_state.GetIsButtonPresed(4) ? 0x00 : 0x10) // P1 A
                 | (_state.GetIsButtonPresed(5) ? 0x00 : 0x20) // P1 B
                 | (_state.GetIsButtonPresed(8) ? 0x00 : 0x40) // P2 Up
                 | (_state.GetIsButtonPresed(9) ? 0x00 : 0x80) // P2 Down
                );
        }

        public void WriteGamepadPort1(Byte value)
        {
            
        }

        public Byte ReadGamepadPort2()
        {
            return (Byte)(
                   (_state.GetIsButtonPresed(10) ? 0x00 : 0x01) // P2 Left
                 | (_state.GetIsButtonPresed(11) ? 0x00 : 0x02) // P2 Right
                 | (_state.GetIsButtonPresed(12) ? 0x00 : 0x04) // P2 A
                 | (_state.GetIsButtonPresed(13) ? 0x00 : 0x08) // P2 B
                 | 0x10                                         // Reset
                 | 0x20                                         // unused
                 | 0x40                                         // Lightgun 1
                 | 0x80                                         // Lightgun 2
                );
        }

        public void WriteGamepadPort2(Byte value)
        {
        }

        public void WriteNationalisation(Byte value)
        {

        }
    }
}
