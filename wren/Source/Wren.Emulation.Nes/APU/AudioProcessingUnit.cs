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
using System.Windows.Forms;
using SlimDX;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using Wren.Emulation.Nes;
namespace AHD.MyNes.Nes
{
    /// <summary>
    /// The class of apu
    /// </summary>
    public class AudioProcessingUnit
    {
        SystemBus _systemBus;
        NesEmulator _Nes;
        IntPtr _handle;
        //slimdx
        DirectSound _SoundDevice;
        SecondarySoundBuffer buffer;
        //Channels
        Chn_Rectangle1 _Chn_REC1;
        Chn_Rectangle2 _Chn_REC2;
        Chn_Triangle _Chn_TRL;
        Chn_Noise _Chn_NOZ;
        Chn_DMC _Chn_DMC;
        bool _Enabled_REC1 = true;
        bool _Enabled_REC2 = true;
        bool _Enabled_TRL = true;
        bool _Enabled_NOZ = true;
        bool _Enabled_DMC = true;
        //APU
        int _FrameCounter = 0;
        bool _PAL;
        bool _FrameIRQ;
        bool _FirstRender = true;
        bool IsPaused;
        public bool IsRendering = false;
        //Buffer
        byte[] DATA = new byte[88200];
        int BufferSize = 88200;
        int W_Pos = 0;//Write position
        int L_Pos = 0;//Last position
        int D_Pos = 0;//Data position
        //Recorder 
        public WaveRecorder RECODER = new WaveRecorder();

        public AudioProcessingUnit(IntPtr handle, NesEmulator NesEmu, SystemBus systemBus)
        {
            _Nes = NesEmu;
            _systemBus = systemBus;
            InitDirectSound(handle);
            _handle = handle;
        }
        void InitDirectSound(IntPtr handle)
        {            
            //Create the device
            _SoundDevice = new DirectSound();
            _SoundDevice.SetCooperativeLevel(handle, CooperativeLevel.Priority);
            //Creat the wav format, it will be mono-44100-pcm-16bit
            //TODO: support more wave formats 
            WaveFormat wav = new WaveFormat();
            wav.FormatTag = WaveFormatTag.Pcm;
            wav.SamplesPerSecond = 44100;
            wav.Channels = 1;//mono
            wav.BitsPerSample = 16;
            wav.AverageBytesPerSecond = 88200;//wav.SamplesPerSecond * wav.Channels * (wav.BitsPerSample / 8);
            wav.BlockAlignment = 2;//(wfx.Channels * wfx.BitsPerSample / 8);
            BufferSize = 88200 * 5;
            //Description
            SoundBufferDescription des = new SoundBufferDescription();
            des.Format = wav;
            des.SizeInBytes = BufferSize;
            des.Flags = BufferFlags.GlobalFocus | BufferFlags.Software;
            //buffer
            buffer = new SecondarySoundBuffer(_SoundDevice, des);
            DATA = new byte[BufferSize];
            buffer.Play(0, PlayFlags.Looping);
            //channels
            InitChannels();
        }
        void InitChannels()
        {
            _Chn_REC1 = new Chn_Rectangle1();
            _Chn_REC2 = new Chn_Rectangle2();
            _Chn_TRL = new Chn_Triangle();
            _Chn_NOZ = new Chn_Noise();
            _Chn_DMC = new Chn_DMC(_Nes, _systemBus);
        }
        public void RenderFrame()
        {
            if (buffer == null | buffer.Disposed | IsRendering)
            { IsRendering = false; return; }
            IsRendering = true;
            _FrameCounter++;
            W_Pos = 0;
            int Seq = _PAL ? _FrameCounter % 5 : FrameCounter % 4;

            #region Update channels depending on Seq tick.
            if (Seq == 0 | Seq == 2)
            {
                _Chn_REC1.UpdateEnvelope();
                _Chn_REC2.UpdateEnvelope();
                _Chn_NOZ.UpdateEnvelope();
                _Chn_TRL.UpdateEnvelope();
            }
            else if (Seq == 1 | Seq == 3)
            {
                _FrameIRQ = true;
                _Chn_REC1.UpdateSweep();
                _Chn_REC1.UpdateEnvelope();
                _Chn_REC1.UpdateLengthCounter();

                _Chn_REC2.UpdateSweep();
                _Chn_REC2.UpdateEnvelope();
                _Chn_REC2.UpdateLengthCounter();

                _Chn_NOZ.UpdateEnvelope();
                _Chn_NOZ.UpdateLengthCounter();

                _Chn_TRL.UpdateEnvelope();
                _Chn_TRL.UpdateEnvelope();
                _Chn_TRL.UpdateLengthCounter();
            }
            #endregion

            #region Write the buffer
            W_Pos = buffer.CurrentWritePosition;
            if (_FirstRender)
            {
                _FirstRender = false;
                D_Pos = buffer.CurrentWritePosition + 0x1000;
                L_Pos = buffer.CurrentWritePosition;
            }

            int po = W_Pos - L_Pos;
            if (po < 0)
            {
                po = (BufferSize - L_Pos) + W_Pos;
            }
            if (po != 0)
            {
                for (int i = 0; i < po; i += 2)
                {
                    short OUT = 0;
                    #region Mix !!
                    //TODO : make a real mixer !!
                    if (_Enabled_REC1)
                        OUT += _Chn_REC1.RenderSample();
                    if (_Enabled_REC2)
                        OUT += _Chn_REC2.RenderSample();
                    if (_Enabled_NOZ)
                        OUT += _Chn_NOZ.RenderSample();
                    if (_Enabled_TRL)
                        OUT += _Chn_TRL.RenderSample();
                    if (_Enabled_DMC)
                        OUT += _Chn_DMC.RenderSample();
                    OUT *= 2;//Level up !!
                    //RECORD !!
                    if (RECODER.IsRecording)
                        RECODER.AddSample(OUT);
                    #endregion
                    //Out !!
                    if (D_Pos + 1 < DATA.Length)
                    {
                        DATA[D_Pos] = (byte)((OUT & 0xFF00) >> 8);
                        DATA[D_Pos + 1] = (byte)(OUT & 0xFF);
                    }
                    D_Pos += 2;
                    D_Pos = D_Pos % BufferSize;
                }
                buffer.Write(DATA, 0, LockFlags.None);
                L_Pos = W_Pos;
            }
            IsRendering = false;
            #endregion
        }
        public void Play()
        {
            if (!IsPaused)
            { return; }
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                IsPaused = false;
                buffer.Play(0, PlayFlags.Looping);
            }
        }
        public void Pause()
        {
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Stop();
                IsPaused = true;
            }
        }
        public void Shutdown()
        {
            if (buffer != null && buffer.Disposed & !IsRendering)
            {
                buffer.Stop(); IsPaused = true;
            }
            while (IsRendering)
            { }
            buffer.Dispose();
            _SoundDevice.Dispose();
            if (RECODER.IsRecording)
                RECODER.Stop();
        }
        #region Registers
        //Rec 1
        public void Write_4000(byte data) { _Chn_REC1.Write_4000(data); }
        public void Write_4001(byte data) { _Chn_REC1.Write_4001(data); }
        public void Write_4002(byte data) { _Chn_REC1.Write_4002(data); }
        public void Write_4003(byte data) { _Chn_REC1.Write_4003(data); }
        //Rec 2
        public void Write_4004(byte data) { _Chn_REC2.Write_4004(data); }
        public void Write_4005(byte data) { _Chn_REC2.Write_4005(data); }
        public void Write_4006(byte data) { _Chn_REC2.Write_4006(data); }
        public void Write_4007(byte data) { _Chn_REC2.Write_4007(data); }
        //Trl 
        public void Write_4008(byte data) { _Chn_TRL.Write_4008(data); }
        public void Write_400A(byte data) { _Chn_TRL.Write_400A(data); }
        public void Write_400B(byte data) { _Chn_TRL.Write_400B(data); }
        //Noz 
        public void Write_400C(byte data) { _Chn_NOZ.Write_400C(data); }
        public void Write_400E(byte data) { _Chn_NOZ.Write_400E(data); }
        public void Write_400F(byte data) { _Chn_NOZ.Write_400F(data); }
        //DMC
        public void Write_4010(byte data) { _Chn_DMC.Write_4010(data); }
        public void Write_4011(byte data) { _Chn_DMC.Write_4011(data); }
        public void Write_4012(byte data) { _Chn_DMC.Write_4012(data); }
        public void Write_4013(byte data) { _Chn_DMC.Write_4013(data); }
        //Status
        public void Write_4015(byte data)
        {
            _Chn_REC1.Enabled = (data & 0x01) != 0;
            _Chn_REC2.Enabled = (data & 0x02) != 0;
            _Chn_TRL.Enabled = (data & 0x04) != 0;
            _Chn_NOZ.Enabled = (data & 0x08) != 0;
            _Chn_DMC.Enabled = (data & 0x10) != 0;
            _FrameIRQ = (data & 0x40) != 0;
            _Chn_DMC.IRQ = true;
        }
        public void Write_4017(byte data)
        {
            _FrameIRQ = (data & 0x40) == 0;
            _PAL = (data & 0x80) != 0;
            _FrameCounter = 0;
        }
        public byte Read_4015()
        {
            byte rt = 0;
            rt |= (byte)(_Chn_REC1.Enabled ? 0x01 : 0);
            rt |= (byte)(_Chn_REC2.Enabled ? 0x02 : 0);
            rt |= (byte)(_Chn_TRL.Enabled ? 0x04 : 0);
            rt |= (byte)(_Chn_NOZ.Enabled ? 0x08 : 0);
            rt |= (byte)(_Chn_DMC.Enabled ? 0x10 : 0);
            rt |= (byte)(_FrameIRQ ? 0x40 : 0);
            rt |= (byte)(_Chn_DMC.IRQ ? 0x80 : 0);
            return rt;
        }
        #endregion
        #region Properties
        public bool Square1Enabled { get { return _Enabled_REC1; } set { _Enabled_REC1 = value; } }
        public bool Square2Enabled { get { return _Enabled_REC2; } set { _Enabled_REC2 = value; } }
        public bool TriangleEnabled { get { return _Enabled_TRL; } set { _Enabled_TRL = value; } }
        public bool NoiseEnabled { get { return _Enabled_NOZ; } set { _Enabled_NOZ = value; } }
        public bool DMCEnabled { get { return _Enabled_DMC; } set { _Enabled_DMC = value; } }
        public int FrameCounter { get { return _FrameCounter; } set { _FrameCounter = value; } }
        public bool PAL { get { return _PAL; } set { _PAL = value; } }
        public bool FrameIRQ { get { return _FrameIRQ; } set { _FrameIRQ = value; } }
        #endregion
    }
}
