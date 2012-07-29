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
    public class Mapper41 : IMapper
    {
        Mappers Map;
        public byte Mapper41_CHR_Low = 0;
        public byte Mapper41_CHR_High = 0;
        public Mapper41(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x6000 & address <= 0xFFFF)
            {
                Map.Switch32kPrgRom((address & 0x7) * 8);
                Map.mapperCartridge.mirroring = ((address & 0x20) == 0) ? MIRRORING.VERTICAL : MIRRORING.HORIZONTAL;
                Mapper41_CHR_High = (byte)(data & 0x18);
                Mapper41_CHR_High <<= 3;
                if ((address & 0x4) == 0)
                {
                    Mapper41_CHR_Low = (byte)(data & 0x3);
                    //Switch8kChrRom(Mapper41_CHR * 8);
                }
                Map.Switch8kChrRom((Mapper41_CHR_High | Mapper41_CHR_Low) * 8);
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
