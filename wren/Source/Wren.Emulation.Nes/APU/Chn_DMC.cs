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
using Wren.Emulation.Nes;

namespace AHD.MyNes.Nes
{
    public class Chn_DMC
    {
        public Chn_DMC(NesEmulator NesEmu, SystemBus bus)
        {
            _Nes = NesEmu;
            _bus = bus;
        }
        NesEmulator _Nes;
        SystemBus _bus;
        bool _IRQ = false;
        bool _Loob;
        int _FreqTimer = 0;
        double _Frequency = 0;
        double _RenderedLength = 0;
        double _SampleCount = 0;
        byte DAC = 0;
        int _Shift = 0;
        ushort _SampleAddress = 0;
        int _SampleLength = 0;
        int _initialLength = 0;
        bool _Enabled = true;
        byte _LengthCount = 0;
        ushort _initialAddress = 0;
        int _dacCounter = 0;
        int[] _dmcWavelengths = new int[] 
        { 
0xD60, 0xBE0, 0xAA0, 0xA00, 0x8F0, 0x7F0, 0x710, 0x6B0, 
0x5F0,0x500, 0x470, 0x400, 0x350, 0x2A8, 0x240, 0x1B0 
        };
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (_LengthCount > 0)
                _LengthCount--;
            _Enabled = _LengthCount != 0;
        }
        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (_SampleCount > _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    if (_SampleLength > 0 & _Shift == 0)
                    {
                        ushort num2;
                        _SampleAddress = (ushort)((num2 = _SampleAddress) + 1);
                        DAC = _bus.ReadMemory8(num2);
                        _SampleLength--;
                        _Shift = 8;
                        if (_Loob & _SampleLength <= 0)
                        {
                            _SampleLength = _initialLength;
                            _SampleAddress = _initialAddress;
                        }
                        if (!_Loob & _SampleLength == 0)
                            _IRQ = true;
                    }
                    if (_SampleLength > 0)
                    {
                        if (DAC != 0)
                        {
                            int num = this.DAC & 1;
                            if ((num == 0) && (_dacCounter > 1))
                            {
                                _dacCounter -= 2;
                            }
                            else if ((num != 0) && (_dacCounter < 0x7e))
                            {
                                _dacCounter += 2;
                            }
                        }
                        _dacCounter--;
                        if (_dacCounter <= 0)
                        {
                            _dacCounter = 8;
                        }
                        DAC = (byte)(DAC >> 1);
                        if (_Shift > 0)
                            _Shift--;
                    }
                }
            }
            return (short)_dacCounter;
        }
        #region Registers
        public void Write_4010(byte data)
        {
            _Loob = (data & 0x40) != 0;
            _FreqTimer = _dmcWavelengths[data & 0xF];
            _IRQ = (data & 0x80) != 0;
            //Update Frequency
            _Frequency = 1790000 / (_FreqTimer + 1) * 8;
            _RenderedLength = 44100 / _Frequency;
        }
        public void Write_4011(byte data)
        {
            DAC = (byte)(data & 0x7f);
            _Shift = 8;
        }
        public void Write_4012(byte data)
        {
            _SampleAddress = (ushort)((data * 0x40) + 0xc000);
            _initialAddress = _SampleAddress;
        }
        public void Write_4013(byte data)
        {
            _SampleLength = (data * 0x10) + 1;
            _initialLength = _SampleLength;
        }
        #endregion
        #region Properties
        public bool IRQ { get { return _IRQ; } set { _IRQ = value; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        #endregion
    }
}
