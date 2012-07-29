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
    public class Chn_Rectangle2
    {
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
        double _DutyPercentage = 0;
        int _FreqTimer = 0;
        bool _Enabled = true;
        byte _DecayCount = 0;
        bool _DecayDiable;
        bool _DecayReset;
        bool _DecayLoopEnable;
        byte _LengthCount = 0;

        byte _SweepShift = 0;
        bool _SweepDirection;
        byte _SweepRate = 0;
        bool _SweepEnable;
        byte _SweepCount = 0;
        bool st;

        public void UpdateLengthCounter()
        {
            //Length Counter
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
            _Enabled = _LengthCount != 0;
        }
        public void UpdateSweep()
        {
            int num = 0;
            if (_SweepCount > 0)
                _SweepCount--;
            if (_SweepEnable & _SweepShift != 0 & _SweepCount == 0 & _LengthCount > 0)
            {
                _SweepCount = _SweepRate;
                if (_FreqTimer >= 8)
                {
                    num = _FreqTimer >> _SweepShift;
                    if (_SweepDirection)
                        num = -num;
                    num += _FreqTimer;
                    if (num > 8 /*& num < 0x800*/)
                    {
                        _FreqTimer = num;
                        //Update Frequency
                        _Frequency = 1790000 / ((_FreqTimer + 1) * 0x10);
                        _RenderedLength = 44100 / _Frequency;
                    }
                    //else
                    //    _SweepEnable = false;
                }
            }
        }
        public void UpdateEnvelope()
        {
            if (_DecayReset)
            {
                _Envelope = 0xF;
                _DecayReset = false;
                if (!_DecayDiable)
                    _Volume = 0xF;
            }
            else
            {
                if (_DecayCount > 0)
                    _DecayCount--;
                else
                {
                    if (_Envelope > 0)
                        _Envelope--;
                    else if (_DecayLoopEnable)
                        _Envelope = 0xF;

                    if (!_DecayDiable)
                        _Volume = _Envelope;
                }
            }
        }
        public byte RenderSample()
        {
            if (_LengthCount > 0 & _Enabled)
            {
                _SampleCount++;
                if (st & _SampleCount >= (_RenderedLength * _DutyPercentage))
                {
                    _SampleCount -= (_RenderedLength * _DutyPercentage);
                    st = !st;
                }
                else if (!st & _SampleCount >= (_RenderedLength * (1.0 - _DutyPercentage)))
                {
                    _SampleCount -= (_RenderedLength * (1.0 - _DutyPercentage));
                    st = !st;
                }
                if (st)
                    return 0;
                return (_DecayDiable ? _Volume : _Envelope);
            }
            return 0;
        }
        #region Registers
        public void Write_4004(byte data)
        {
            _Volume = _DecayCount = (byte)(data & 0xF);//bit 0 - 3
            //_DecayCount++;
            _DecayDiable = ((data & 0x10) != 0);//bit 4
            _DecayLoopEnable = ((data & 0x20) != 0);//bit 5
            if ((data >> 6) == 0)
                _DutyPercentage = 0.125;
            else if ((data >> 6) == 1)
                _DutyPercentage = 0.25;
            else if ((data >> 6) == 2)
                _DutyPercentage = 0.5;
            else if ((data >> 6) == 3)
                _DutyPercentage = 0.75;
        }
        public void Write_4005(byte data)
        {
            _SweepShift = (byte)(data & 0x7);//bit 0 - 2
            _SweepDirection = ((data & 0x8) != 0);//bit 3
            _SweepRate = (byte)((data & 0x70) >> 4);//bit 4 - 6
            _SweepEnable = ((data & 0x80) != 0);//bit 7
        }
        public void Write_4006(byte data)
        {
            _FreqTimer = (ushort)((_FreqTimer & 0x700) | data);
            //Update Frequency
            _Frequency = 1790000 / ((_FreqTimer + 1) * 0x10);
            _RenderedLength = 44100 / _Frequency;
        }
        public void Write_4007(byte data)
        {
            _FreqTimer = (ushort)((_FreqTimer & 0xFF) | (data & 0x7) << 8);//Bit 0 - 2
            _LengthCount = LENGTH_COUNTER_TABLE[(data & 0xf8) >> 3];//bit 3 - 7 
            _DecayReset = true;
            //Update Frequency
            _Frequency = 1790000 / ((_FreqTimer + 1) * 0x10);
            _RenderedLength = 44100 / _Frequency;
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
