using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using Wren.Core;

namespace Wren.Emulation.Nes
{
    public class Joypad
    {
        public static InputState InputState { get; set; }
        // Methods
        public int GetJoyData(InputState inputState, Int32 port)
        {
            if (port == 0)
            {
                int num = 0;

                if (inputState.GetIsButtonPresed(4))
                    num |= 1;

                if (inputState.GetIsButtonPresed(5))
                    num |= 2;

                if (inputState.GetIsButtonPresed(6))
                    num |= 4;

                if (inputState.GetIsButtonPresed(7))
                    num |= 8;

                if (inputState.GetIsButtonPresed(0))
                    num |= 0x10;

                if (inputState.GetIsButtonPresed(1))
                    num |= 0x20;

                if (inputState.GetIsButtonPresed(2))
                    num |= 0x40;

                if (inputState.GetIsButtonPresed(3))
                    num |= 0x80;

                return num;
            }
            else
            {
                int num = 0;

                if (inputState.GetIsButtonPresed(12))
                    num |= 1;

                if (inputState.GetIsButtonPresed(13))
                    num |= 2;

                if (inputState.GetIsButtonPresed(14))
                    num |= 4;

                if (inputState.GetIsButtonPresed(15))
                    num |= 8;

                if (inputState.GetIsButtonPresed(8))
                    num |= 0x10;

                if (inputState.GetIsButtonPresed(9))
                    num |= 0x20;

                if (inputState.GetIsButtonPresed(10))
                    num |= 0x40;

                if (inputState.GetIsButtonPresed(11))
                    num |= 0x80;

                return num;
            }
        }
    }
}

