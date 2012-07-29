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
    public class Chn_Noise
    {
        ushort[] NOISE_FREQUENCY_TABLE = 
        {
0x2,0x4,0x8,0x10,0x20,0x30,0x40,0x50,0x65,
0x7f,0xbe,0xfe,0x17D,0x1fc,0x3f9,0x7f2
        };
        byte[] LENGTH_COUNTER_TABLE = 
        {
0x5*2,0x7f*2,0xA*2,0x1*2,0x14*2,0x2*2,0x28*2,0x3*2,0x50*2,0x4*2,0x1E*2,0x5*2,0x7*2,0x6*2,0x0E*2,0x7*2,
0x6*2,0x08*2,0xC*2,0x9*2,0x18*2,0xa*2,0x30*2,0xb*2,0x60*2,0xc*2,0x24*2,0xd*2,0x8*2,0xe*2,0x10*2,0xf*2
        };
        byte _Volume = 0;
        byte _Envelope = 0;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        int _FreqTimer = 0;
        byte _LengthCount = 0;
        int _ShiftReg = 1;
        int OUT = 0;
        byte _DecayCount = 0;
        bool _DecayDiable;
        bool _NoiseMode;
        bool _DecayLoopEnable;
        bool _Enabled = true;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
            _Enabled = _LengthCount != 0;
        }
        /// <summary>
        /// Update Envelope / Decay / Linear Counter
        /// </summary>
        public void UpdateEnvelope()
        {
            if (_DecayCount > 0 & _DecayLoopEnable)
                _DecayCount--;
            if (_DecayCount == 0 & _Envelope > 0 & !_DecayDiable)
            {
                _Envelope--;
                //_DecayCount = _Volume;
            }
            if (_DecayLoopEnable & _Envelope == 0)
            {
                _Envelope = 0xF;
            }
        }
        //Do NOZ samples
        public byte RenderSample()
        {
            if (_LengthCount > 0)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    int sh = (_ShiftReg & 0x4000) >> 14;//Bit14
                    int sh1;
                    if (_NoiseMode)
                    {
                        sh1 = (_ShiftReg & 0x0100) >> 8;//Bit8
                    }
                    else
                    {
                        sh1 = (_ShiftReg & 0x2000) >> 13;//Bit13
                    }
                    _ShiftReg <<= 1;
                    _ShiftReg |= (sh ^ sh1) & 1;
                }
                OUT = (_ShiftReg & 1) * 0x20;
                OUT *= (_DecayDiable ? _Volume : _Envelope);
                return (byte)(OUT >> 5);
            }
            return 0;
        }

        #region Registerts
        public void Write_400C(byte data)
        {
            _Volume = _DecayCount = (byte)(data & 0xF);//bit 0 - 3
            _DecayDiable = ((data & 0x10) != 0);//bit 4
            _DecayLoopEnable = ((data & 0x20) != 0);//bit 5
        }
        public void Write_400E(byte data)
        {
            _FreqTimer = NOISE_FREQUENCY_TABLE[data & 0x0F];//bit 0 - 3
            _NoiseMode = ((data & 0x80) != 0);//bit 7
            //Update Frequency
            _Frequency = 1790000 / 2 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        public void Write_400F(byte data)
        {
            _LengthCount = LENGTH_COUNTER_TABLE[(data >> 3) & 0x1F];//bit 3 - 7
            _DecayCount = 0xF;//Reset the decay counter
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                if (!value)
                    _LengthCount = 0;
            }
        }
        #endregion
    }
}
