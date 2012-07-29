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
    public class Mapper5 : IMapper
    {
        Mappers Map;
        public byte mapper5_prgBankSize;
        public byte mapper5_chrBankSize;
        public int mapper5_scanlineSplit;
        public bool mapper5_splitIrqEnabled;
        public Mapper5(Mappers Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {

            if (address == 0x5100)
            {
                mapper5_prgBankSize = (byte)(data & 0x3);
            }
            else if (address == 0x5101)
            {
                mapper5_chrBankSize = (byte)(data & 0x3);
            }
            else if (address == 0x5105)
            {
            }
            else if (address == 0x5114)
            {
                if (mapper5_prgBankSize == 3)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 0);
                }
            }
            else if (address == 0x5115)
            {
                if (mapper5_prgBankSize == 1)
                {
                    Map.Switch16kPrgRom((data & 0x7e) * 2, 0);
                }
                else if (mapper5_prgBankSize == 2)
                {
                    Map.Switch16kPrgRom((data & 0x7e) * 2, 0);
                }
                else if (mapper5_prgBankSize == 3)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 1);
                }
            }
            else if (address == 0x5116)
            {
                if (mapper5_prgBankSize == 2)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                }
                else if (mapper5_prgBankSize == 3)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                }
            }
            else if (address == 0x5117)
            {
                if (mapper5_prgBankSize == 0)
                {
                    Map.Switch32kPrgRom((data & 0x7c) * 2);
                }
                else if (mapper5_prgBankSize == 1)
                {
                    Map.Switch16kPrgRom((data & 0x7e) * 2, 1);
                }
                else if (mapper5_prgBankSize == 2)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                }
                else if (mapper5_prgBankSize == 3)
                {
                    Map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                }

            }
            else if (address == 0x5120)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 0);
                }
            }
            else if (address == 0x5121)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 1);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 0);
                }
            }
            else if (address == 0x5122)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 2);
                }
            }
            else if (address == 0x5123)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 3);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 1);
                }
                else if (mapper5_chrBankSize == 1)
                {
                    Map.Switch4kChrRom(data, 0);
                }
            }
            else if (address == 0x5124)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 4);
                }
            }
            else if (address == 0x5125)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 5);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 2);
                }
            }
            else if (address == 0x5126)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 6);
                }
            }
            else if (address == 0x5127)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 7);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 3);
                }
                else if (mapper5_chrBankSize == 1)
                {
                    Map.Switch4kChrRom(data, 1);
                }
                else if (mapper5_chrBankSize == 0)
                {
                    Map.Switch8kChrRom(data);
                }
            }
            else if (address == 0x5128)
            {
                Map.Switch1kChrRom(data, 0);
                Map.Switch1kChrRom(data, 4);
            }
            else if (address == 0x5129)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 1);
                    Map.Switch1kChrRom(data, 5);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 0);
                    Map.Switch2kChrRom(data, 2);
                }
            }
            else if (address == 0x512a)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 2);
                    Map.Switch1kChrRom(data, 6);
                }
            }
            else if (address == 0x512b)
            {
                if (mapper5_chrBankSize == 3)
                {
                    Map.Switch1kChrRom(data, 3);
                    Map.Switch1kChrRom(data, 7);
                }
                else if (mapper5_chrBankSize == 2)
                {
                    Map.Switch2kChrRom(data, 1);
                    Map.Switch2kChrRom(data, 3);
                }
                else if (mapper5_chrBankSize == 1)
                {
                    Map.Switch4kChrRom(data, 0);
                    Map.Switch4kChrRom(data, 1);
                }
                else if (mapper5_chrBankSize == 0)
                {
                    Map.Switch8kChrRom(data);
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 0);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 1);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 2);
            Map.Switch8kPrgRom((Map.mapperCartridge.prg_rom_pages * 4) - 2, 3);
            mapper5_splitIrqEnabled = false;
            Map.Switch8kChrRom(0);
        }
        public void TickTimer()
        {

        }
    }
}