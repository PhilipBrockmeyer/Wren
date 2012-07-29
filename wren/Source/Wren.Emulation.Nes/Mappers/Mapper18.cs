//////////////////////////////////////////////////////////////////////////////
//This file is part of My Nes                                               //
//A Nintendo Entertainment System Emulator.                                 //
//                                                                          //
//Copyright © 2009 - 2010 Ala Hadid (AHD)                                   //
//                                                                          //
//My Nes is free software; you can redistribute it and/or modify            //
//it under the terms of the GNU General Public License as published by      //
//the Free Software Foundation; either version 2 of the License, or         //
//(at your option) any later version.                                       //
//                                                                          //
//My Nes is distributed in the hope that it will be useful,                 //
//but WITHOUT ANY WARRANTY; without even the implied warranty of            //
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the             //
//GNU General Public License for more details.                              //
//                                                                          //
//You should have received a copy of the GNU General Public License         //
//along with this program; if not, write to the Free Software               //
//Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA//
//////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AHD.MyNes.Nes
{
    [Serializable()]
    public class Mapper18 : IMapper
    {
        Mappers Map;
        SystemBus _bus;
        public ushort Mapper18_Timer = 0;
        public ushort Mapper18_latch = 0;
        public byte mapper18_control = 0;
        public int Mapper18_IRQWidth = 0;
        public bool timer_irq_enabled;
        public byte[] x = new byte[22];
        public Mapper18(Mappers Maps, SystemBus bus)
        { 
            Map = Maps;
            _bus = bus;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: x[0] = 0; x[0] = (byte)((data & 0x0f)); break;
                case 0x8001: x[1] = 0; x[1] = (byte)(((data & 0x0f) << 4) | x[0]); Map.Switch8kPrgRom((x[1]) * 2, 0); break;
                case 0x8002: x[2] = 0; x[2] = (byte)((data & 0x0f)); break;
                case 0x8003: x[3] = 0; x[3] = (byte)(((data & 0x0f) << 4) | x[2]); Map.Switch8kPrgRom((x[3]) * 2, 1); break;
                case 0x9000: x[4] = 0; x[4] = (byte)((data & 0x0f)); break;
                case 0x9001: x[5] = 0; x[5] = (byte)(((data & 0x0f) << 4) | x[4]); Map.Switch8kPrgRom((x[5]) * 2, 2); break;
                case 0x9002:
                    Map.mapperCartridge.save_ram_present = ((data & 0x1) != 0);
                    Map.myEngine.isSaveRAMReadonly = !Map.mapperCartridge.save_ram_present;
                    break;
                case 0xA000: x[3] &= 0xf0; x[3] |= (byte)((data & 0x0f)); break;
                case 0xA001: x[3] &= 0x0f; x[3] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[3]), 0); break;
                case 0xA002: x[4] &= 0xf0; x[4] |= (byte)((data & 0x0f)); break;
                case 0xA003: x[4] &= 0x0f; x[4] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[4]), 1); break;
                case 0xB000: x[5] &= 0xf0; x[5] |= (byte)((data & 0x0f)); break;
                case 0xB001: x[5] &= 0x0f; x[5] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[5]), 2); break;
                case 0xB002: x[6] &= 0xf0; x[6] |= (byte)((data & 0x0f)); break;
                case 0xB003: x[6] &= 0x0f; x[6] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[6]), 3); break;
                case 0xC000: x[7] &= 0xf0; x[7] |= (byte)((data & 0x0f)); break;
                case 0xC001: x[7] &= 0x0f; x[7] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[7]), 4); break;
                case 0xC002: x[8] &= 0xf0; x[8] |= (byte)((data & 0x0f)); break;
                case 0xC003: x[8] &= 0x0f; x[8] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[8]), 5); break;
                case 0xD000: x[9] &= 0xf0; x[9] |= (byte)((data & 0x0f)); break;
                case 0xD001: x[9] &= 0x0f; x[9] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[9]), 6); break;
                case 0xD002: x[10] &= 0xf0; x[10] |= (byte)((data & 0x0f)); break;
                case 0xD003: x[10] &= 0x0f; x[10] |= (byte)((data & 0x0f) << 4); Map.Switch1kChrRom((x[10]), 7); /*x[10] = 0xff;*/ break;
                case 0xE000: Mapper18_latch &= 0xFFF0; Mapper18_latch |= (byte)((data & 0x0f)); break;
                case 0xE001: Mapper18_latch &= 0xFF0f; Mapper18_latch |= (byte)((data & 0x0f) << 4); break;
                case 0xE002: Mapper18_latch &= 0xF0Ff; Mapper18_latch |= (byte)((data & 0x0f) << 8); break;
                case 0xE003: Mapper18_latch &= 0x0FFf; Mapper18_latch |= (byte)((data & 0x0f) << 12); break;
                case 0xF000: timer_irq_enabled = ((data & 0x01) == 0);
                    break;
                case 0xF001:
                    timer_irq_enabled = ((data & 0x01) != 0);
                    Mapper18_IRQWidth = (data & 0x0E);
                    break;
                case 0xF002:
                    int BankMode = (address & 0x3);
                    switch (BankMode)
                    {
                        case 0:
                            Map.mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
                            break;
                        case 1:
                            Map.mapperCartridge.mirroring = MIRRORING.VERTICAL;
                            break;
                        case 2:
                        case 3:
                            Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                            Map.mapperCartridge.mirroringBase = 0x2000;
                            break;
                    } break;
            }
            switch (Mapper18_IRQWidth)
            {
                case 0://16 bit
                    break;
                case 1://12 bit
                    Mapper18_Timer &= 0x0FFF;
                    break;
                case 2:
                case 3://8 bit
                    Mapper18_Timer &= 0x00FF;
                    break;
                case 4:
                case 5:
                case 6:
                case 7://4 bit
                    Mapper18_Timer &= 0x000F;
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Mapper18_latch = 0x0000;
            Mapper18_Timer = 0x0000;
            timer_irq_enabled = false;
            Mapper18_Timer = 0x0000;
            Mapper18_latch = 0x0000;
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.mapperCartridge.prg_rom_pages - 1) * 4, 1);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {
            if (timer_irq_enabled)
            {
                if (Mapper18_Timer == 0)
                {
                    Map.myEngine.my6502.Push16(Map.myEngine.my6502.pc_register);
                    Map.myEngine.my6502.PushStatus();
                    Map.myEngine.my6502.pc_register = _bus.ReadMemory16(0xFFFE);
                    //myEngine.my6502.interrupt_flag = 1;
                    timer_irq_enabled = false;
                    //Mapper18_Timer = Mapper18_latch;
                }
                else
                    Mapper18_Timer -= Mapper18_latch;
            }
        }
    }
}
