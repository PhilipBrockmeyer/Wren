﻿//////////////////////////////////////////////////////////////////////////////
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
    public class Mapper64 : IMapper
    {
        Mappers Map;
        public byte mapper64_commandNumber;
        public byte mapper64_prgAddressSelect;
        public byte mapper64_chrAddressSelect;

        public Mapper64(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                mapper64_commandNumber = data;
                mapper64_prgAddressSelect = (byte)(data & 0x40);
                mapper64_chrAddressSelect = (byte)(data & 0x80);
            }
            else if (address == 0x8001)
            {
                if ((mapper64_commandNumber & 0xf) == 0)
                {
                    //Swap 2 1k chr roms
                    data = (byte)(data - (data % 2));
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch2kChrRom(data, 0);
                    }
                    else
                    {
                        Map.Switch2kChrRom(data, 2);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 1)
                {
                    //Swap 2 1k chr roms
                    data = (byte)(data - (data % 2));
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch2kChrRom(data, 1);
                    }
                    else
                    {
                        Map.Switch2kChrRom(data, 3);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 2)
                {
                    //Swap 1k chr rom
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 4);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 0);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 3)
                {
                    //Swap 1k chr rom
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 5);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 1);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 4)
                {
                    //Swap 1k chr rom
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 6);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 2);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 5)
                {
                    //Swap 1k chr rom
                    if (mapper64_chrAddressSelect == 0)
                    {
                        Map.Switch1kChrRom(data, 7);
                    }
                    else
                    {
                        Map.Switch1kChrRom(data, 3);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 6)
                {
                    if (mapper64_prgAddressSelect == 0)
                    {
                        Map.Switch8kPrgRom(data * 2, 0);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(data * 2, 1);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 7)
                {
                    if (mapper64_prgAddressSelect == 0)
                    {
                        Map.Switch8kPrgRom(data * 2, 1);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(data * 2, 2);
                    }
                }
                else if ((mapper64_commandNumber & 0xf) == 8)
                {
                    Map.Switch1kChrRom(data, 1);
                }
                else if ((mapper64_commandNumber & 0xf) == 9)
                {
                    Map.Switch1kChrRom(data, 3);
                }
                else if ((mapper64_commandNumber & 0xf) == 0xf)
                {
                    if (mapper64_prgAddressSelect == 0)
                    {
                        Map.Switch8kPrgRom(data * 2, 2);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(data * 2, 0);
                    }
                }
            }
            else if (address == 0xA000)
            {
                if ((data & 1) == 1)
                {
                    Map.mapperCartridge.mirroring = MIRRORING.VERTICAL;
                }
                else
                {
                    Map.mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
                }

            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 0);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 1);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 2);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 3);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {

        }
    }
}
