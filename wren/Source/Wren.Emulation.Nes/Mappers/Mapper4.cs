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
    public class Mapper4 : IMapper
    {
        Mappers Map;
        SystemBus _bus;
        public int mapper4_commandNumber;
        public int mapper4_prgAddressSelect;
        public int mapper4_chrAddressSelect;
        public bool timer_irq_enabled;
        public uint timer_irq_count, timer_irq_reload;
        public Mapper4(Mappers Maps, SystemBus bus)
        { 
            Map = Maps;
            _bus = bus;
        }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                mapper4_commandNumber = data & 0x7;
                mapper4_prgAddressSelect = data & 0x40;
                mapper4_chrAddressSelect = data & 0x80;
            }
            else if (address == 0x8001)
            {
                if (mapper4_commandNumber == 0)
                {

                    data = (byte)(data - (data % 2));
                    if (mapper4_chrAddressSelect == 0)
                        Map.Switch2kChrRom(data, 0);
                    else
                        Map.Switch2kChrRom(data, 2);
                }
                else if (mapper4_commandNumber == 1)
                {

                    data = (byte)(data - (data % 2));
                    if (mapper4_chrAddressSelect == 0)
                    {
                        Map.Switch2kChrRom(data, 1);
                    }
                    else
                    {
                        Map.Switch2kChrRom(data, 3);
                    }
                }
                else if (mapper4_commandNumber == 2)
                {

                    data = (byte)(data & (Map.mapperCartridge.chr_rom_pages * 8 - 1));
                    if (mapper4_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 4);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 0);
                    }
                }
                else if (mapper4_commandNumber == 3)
                {

                    if (mapper4_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 5);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 1);
                    }
                }
                else if (mapper4_commandNumber == 4)
                {

                    if (mapper4_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 6);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 2);
                    }
                }
                else if (mapper4_commandNumber == 5)
                {

                    if (mapper4_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 7);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 3);
                    }
                }
                else if (mapper4_commandNumber == 6)
                {
                    if (mapper4_prgAddressSelect == 0)
                    {
                        Map.Switch8kPrgRom(data * 2, 0);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(data * 2, 2);
                    }
                }
                else if (mapper4_commandNumber == 7)
                {

                    Map.Switch8kPrgRom(data * 2, 1);
                }

                if (mapper4_prgAddressSelect == 0)
                { Map.Switch8kPrgRom(((Map.mapperCartridge.prg_rom_pages * 4) - 2) * 2, 2); }
                else
                { Map.Switch8kPrgRom(((Map.mapperCartridge.prg_rom_pages * 4) - 2) * 2, 0); }
                Map.Switch8kPrgRom(((Map.mapperCartridge.prg_rom_pages * 4) - 1) * 2, 3);
            }
            else if (address == 0xA000)
            {
                if ((data & 0x1) == 0)
                {
                    Map.mapperCartridge.mirroring = MIRRORING.VERTICAL;
                }
                else
                {
                    Map.mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
                }
            }
            else if (address == 0xA001)
            {
                if ((data & 0x80) == 0)
                    Map.myEngine.isSaveRAMReadonly = true;
                else
                    Map.myEngine.isSaveRAMReadonly = false;
            }
            else if (address == 0xC000)
            { timer_irq_reload = data; }
            else if (address == 0xC001)
            { timer_irq_count = data; }
            else if (address == 0xE000)
            {
                timer_irq_enabled = false;
                timer_irq_reload = timer_irq_count;
            }
            else if (address == 0xE001)
            { timer_irq_enabled = true; }
        }
        public void SetUpMapperDefaults()
        {
            timer_irq_count = timer_irq_reload = 0xff;
            mapper4_prgAddressSelect = 0;
            mapper4_chrAddressSelect = 0;
            Map.Switch16kPrgRom((Map.mapperCartridge.prg_rom_pages - 1) * 4, 1);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {
            if (timer_irq_reload == 0)
            {
                if (timer_irq_enabled)
                {
                    Map.myEngine.my6502.brk_flag = 0;
                    Map.myEngine.my6502.Push16(Map.myEngine.my6502.pc_register);
                    Map.myEngine.my6502.PushStatus();
                    Map.myEngine.my6502.interrupt_flag = 1;
                    Map.myEngine.my6502.pc_register = _bus.ReadMemory16(0xFFFE);
                    timer_irq_enabled = false;
                }
                timer_irq_reload = timer_irq_count;
            }
            timer_irq_reload -= 1;
        }
    }
}
