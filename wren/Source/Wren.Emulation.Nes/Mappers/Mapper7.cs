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
    internal class Mapper7 : IMapper
    {
        Mappers Map;
        public Mapper7(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                Map.Switch32kPrgRom((data & 0xf) * 8);
                if ((data & 0x10) == 0x10)
                {
                    Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                    Map.mapperCartridge.mirroringBase = 0x2400;
                }
                else
                {
                    Map.mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
                    Map.mapperCartridge.mirroringBase = 0x2000;
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
