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
    internal class Mapper15 : IMapper
    {
        Mappers Map;
        public Mapper15(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                byte X = (byte)(data & 0x3F);
                Map.mapperCartridge.mirroring = ((data & 0x40) == 0) ? MIRRORING.VERTICAL : MIRRORING.HORIZONTAL;
                byte Y = (byte)(data & 0x80);
                Y >>= 7;
                switch (address & 0x3)
                {
                    case 0://0=32K
                        Map.Switch16kPrgRom(X * 4, 0);
                        Map.Switch16kPrgRom((X + 1) * 4, 1);
                        break;
                    case 1://1=128K
                        Map.Switch16kPrgRom(X * 4, 0);
                        Map.Switch16kPrgRom((Map.mapperCartridge.prg_rom_pages - 1) * 4, 1);
                        break;
                    case 2://2=8K
                        Map.Switch8kPrgRom(((X * 2) + Y) * 2, 0);
                        Map.Switch8kPrgRom(((X * 2) + Y) * 2, 1);
                        Map.Switch8kPrgRom(((X * 2) + Y) * 2, 2);
                        Map.Switch8kPrgRom(((X * 2) + Y) * 2, 3);
                        break;
                    case 3://3=16K
                        Map.Switch16kPrgRom(X * 4, 0);
                        Map.Switch16kPrgRom(X * 4, 1);
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch32kPrgRom(0);
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {

        }
    }
}
