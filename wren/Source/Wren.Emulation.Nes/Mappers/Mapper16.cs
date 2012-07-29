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
    public class Mapper16 : IMapper
    {
        Mappers Map;
        SystemBus _bus;
        public ushort timer_irq_counter_16 = 0;
        public ushort timer_irq_Latch_16 = 0;
        public bool timer_irq_enabled;
        
        public Mapper16(Mappers Maps, SystemBus bus)
        { 
            Map = Maps;
            _bus = bus;
        }
        public void Write(ushort address, byte data)
        {
            switch (address & 0x000F)
            {
                case 0: Map.Switch1kChrRom(data, 0); break;
                case 1: Map.Switch1kChrRom(data, 1); break;
                case 2: Map.Switch1kChrRom(data, 2); break;
                case 3: Map.Switch1kChrRom(data, 3); break;
                case 4: Map.Switch1kChrRom(data, 4); break;
                case 5: Map.Switch1kChrRom(data, 5); break;
                case 6: Map.Switch1kChrRom(data, 6); break;
                case 7: Map.Switch1kChrRom(data, 7); break;
                case 8: Map.Switch16kPrgRom(data * 4, 0); break;
                case 9: switch (data & 0x3)
                    {
                        case 0:
                            Map.mapperCartridge.mirroring = MIRRORING.VERTICAL;
                            break;
                        case 1:
                            Map.mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
                            break;
                        case 2:
                            Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                            Map.mapperCartridge.mirroringBase = 0x2000;
                            break;
                        case 3:
                            Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                            Map.mapperCartridge.mirroringBase = 0x2400;
                            break;
                    } break;
                case 0xA: timer_irq_enabled = ((data & 0x1) != 0);
                    break;
                case 0xB:
                    //lowbyte
                    timer_irq_Latch_16 &= 0xFF00; timer_irq_Latch_16 |= data;
                    break;
                case 0xC:
                    //highbyte
                    timer_irq_Latch_16 &= 0xFF; timer_irq_Latch_16 |= (byte)(data << 8);
                    break;
                case 0xD: break;//
            }
        }
        public void SetUpMapperDefaults()
        {
            timer_irq_counter_16 = 0x0000;
            timer_irq_Latch_16 = 0x0000;
            timer_irq_enabled = false;
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.mapperCartridge.prg_rom_pages - 1) * 4, 1);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {
            if (timer_irq_enabled)
            {
                if (timer_irq_counter_16 == 0)
                {
                    Map.myEngine.my6502.Push16(Map.myEngine.my6502.pc_register);
                    Map.myEngine.my6502.PushStatus();
                    Map.myEngine.my6502.pc_register = _bus.ReadMemory16(0xFFFE);
                    Map.myEngine.my6502.interrupt_flag = 1;
                    timer_irq_enabled = false;
                    //timer_irq_counter_16 = 0xFFFF;
                    timer_irq_counter_16 = timer_irq_Latch_16;
                }
                else
                { timer_irq_counter_16 -= 1; }
            }
        }
    }
}