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
    internal class Mapper71 : IMapper
    {
        Mappers Map;
        public Mapper71(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0xF000:
                case 0xE000:
                case 0xD000:
                case 0xC000: Map.Switch16kPrgRom(data * 4, 0); break;
                case 0x9000:
                    Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                    if ((data & 0x10) != 0)
                    {
                        Map.mapperCartridge.mirroringBase = 0x2000;
                    }
                    else
                    {
                        Map.mapperCartridge.mirroringBase = 0x2400;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.mapperCartridge.prg_rom_pages - 1) * 4, 1);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {

        }
    }
}
