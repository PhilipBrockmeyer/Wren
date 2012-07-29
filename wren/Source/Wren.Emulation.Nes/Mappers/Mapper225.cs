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
    /* by AHD.
     * Same as 255 for X in 1 carts
     * 100% !!
     */
    [Serializable()]
    public class Mapper225 : IMapper
    {
        Mappers Map;
        public byte Mapper225_reg0 = 0xF;
        public byte Mapper225_reg1 = 0xF;
        public byte Mapper225_reg2 = 0xF;
        public byte Mapper225_reg3 = 0xF;
        public Mapper225(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                int banks = ((address & 0x4000) != 0) ? 1 : 0;
                ushort wher = (ushort)((address & 0x0Fc0) >> 7);
                Map.Switch8kChrRom(((address & 0x003F) + (banks << 6)) * 8);
                if ((address & 0x1000) != 0)//A12
                {
                    if ((address & 0x0040) != 0)//A6
                    {
                        Map.Switch16kPrgRom((((wher + (banks << 5)) << 1) + 1) * 4, 0);
                        Map.Switch16kPrgRom((((wher + (banks << 5)) << 1) + 1) * 4, 1);
                    }
                    else
                    {
                        Map.Switch16kPrgRom(((wher + (banks << 5)) << 1) * 4, 0);
                        Map.Switch16kPrgRom(((wher + (banks << 5)) << 1) * 4, 1);
                    }
                }
                else
                {
                    Map.Switch32kPrgRom((wher + (banks << 5)) * 8);//ignore A6
                }
                Map.mapperCartridge.mirroring = ((address >> 13) & 1) == 0 ? MIRRORING.VERTICAL : MIRRORING.HORIZONTAL;
            }
            if ((address >= 0x5800) && (address <= 0x5FFF))
            {
                switch (address & 0x3)
                {
                    case 0: Mapper225_reg0 = (byte)(data & 0xf); break;
                    case 1: Mapper225_reg1 = (byte)(data & 0xf); break;
                    case 2: Mapper225_reg2 = (byte)(data & 0xf); break;
                    case 3: Mapper225_reg3 = (byte)(data & 0xf); break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch32kPrgRom(0);
            Map.mapperCartridge.mirroring = MIRRORING.VERTICAL;
            Map.Switch8kChrRom(0);
            Mapper225_reg0 = 0xF;
            Mapper225_reg1 = 0xF;
            Mapper225_reg2 = 0xF;
            Mapper225_reg3 = 0xF;
        }
        public void TickTimer()
        {

        }
    }
}
