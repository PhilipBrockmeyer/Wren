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
    public class Chn_Triangle
    {
        byte[] LENGTH_COUNTER_TABLE = 
        {
0x5*2,0x7f*2,0xA*2,0x1*2,0x14*2,0x2*2,0x28*2,0x3*2,0x50*2,0x4*2,0x1E*2,0x5*2,0x7*2,0x6*2,0x0E*2,0x7*2,
0x6*2,0x08*2,0xC*2,0x9*2,0x18*2,0xa*2,0x30*2,0xb*2,0x60*2,0xc*2,0x24*2,0xd*2,0x8*2,0xe*2,0x10*2,0xf*2
        };
        byte[] TRIANGL_SEQUENCE = new byte[] 
        { 
0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0
        };
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        int _FreqTimer = 0;
        byte _LengthCount = 0;
        int _LinearCounter = 0;
        int _LinearCounterLoad = 0;
        bool _LinearCounterDisable;
        uint _Sequence = 0;
        bool HALT;
        byte OUT;
        bool _Enabled = true;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (_LinearCounterDisable & _LengthCount > 0)
                _LengthCount--;
            _Enabled = _LengthCount != 0;
        }
        //Envelope / Decay / Linear Counter
        /// <summary>
        /// Update Envelope / Decay / Linear Counter
        /// </summary>
        public void UpdateEnvelope()
        {
            if (HALT)
                _LinearCounter = _LinearCounterLoad;
            else if (_LinearCounter > 0 & _LinearCounterDisable)
            {
                _LinearCounter--;
            }

            if (_LinearCounterDisable)
                HALT = false;
        }
        /// <summary>
        /// Do TRL sample
        /// </summary>
        /// <returns></returns>
        public byte RenderSample()
        {
            if (_LinearCounter > 0 & _LengthCount > 0 & _FreqTimer >= 8)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    _Sequence++;
                }
                OUT = TRIANGL_SEQUENCE[_Sequence & 0x1f];
                return OUT;
            }
            if (OUT > 0)
                OUT--;
            return OUT;
        }
        #region Registers
        public void Write_4008(byte data)
        {
            _LinearCounterLoad = (byte)(data & 0x7F);//Bit 0 - 6
            _LinearCounterDisable = (data & 0x80) == 0;//Bit 7
        }
        public void Write_400A(byte data)
        {
            _FreqTimer = (ushort)((_FreqTimer & 0x700) | data);
            //Update Frequency
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
        }
        public void Write_400B(byte data)
        {
            _FreqTimer = (ushort)((_FreqTimer & 0xFF) | (data & 0x7) << 8);//Bit 0 - 2
            //Update Frequency
            _Frequency = 1790000 / (_FreqTimer + 1);
            _RenderedLength = 44100 / _Frequency;
            _LengthCount = LENGTH_COUNTER_TABLE[(data & 0xf8) >> 3];//bit 3 - 7 
            HALT = true;
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
