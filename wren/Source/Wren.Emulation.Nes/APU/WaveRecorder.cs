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
using System.IO;

namespace AHD.MyNes.Nes
{
    /* Written by AHD at Wednesday, January 13, 2010.
     * Wave recorder for PCM/16-BIT/MONO/44100Hz
     * Written this for debugging the sound
     * So that you can open generated audio in
     * Wave programs
     * TODO : support for more audio formats.
     */
    public class WaveRecorder
    {
        Stream STR;
        public bool IsRecording = false;
        int SIZE = 0;
        int NoOfSamples = 0;
        public void Record(string FilePath)
        {
            //Create the stream first
            STR = new FileStream(FilePath, FileMode.Create);
            ASCIIEncoding ASCII = new ASCIIEncoding();
            //1 Write the header "RIFF"
            STR.Write(ASCII.GetBytes("RIFF"), 0, 4);
            //2 Write Chunck Size (0 for now)
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //3 Write WAVE
            STR.Write(ASCII.GetBytes("WAVE"), 0, 4);
            //4 Write "fmt "
            STR.Write(ASCII.GetBytes("fmt "), 0, 4);
            //5 Write Chunck Size (16 for PCM)
            STR.WriteByte(0x10);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //6 Write audio format (1 = PCM)
            STR.WriteByte(0x01);
            STR.WriteByte(0x00);
            //7 Number of channels (Mono = 1)
            STR.WriteByte(0x01);
            STR.WriteByte(0x00);
            //8 Sample Rate (44100)
            STR.WriteByte(0x44);
            STR.WriteByte(0xAC);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //9 Byte Rate (88200)
            STR.WriteByte(0x88);
            STR.WriteByte(0x58);
            STR.WriteByte(0x01);
            STR.WriteByte(0x00);
            //10 Block Align (2)
            STR.WriteByte(0x02);
            STR.WriteByte(0x00);
            //11 Bits Per Sample (16)
            STR.WriteByte(0x10);
            STR.WriteByte(0x00);
            //12 Write "data"
            STR.Write(ASCII.GetBytes("data"), 0, 4);
            //13 Write Chunck Size (0 for now)
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //Confirm
            IsRecording = true;
        }
        public void AddSample(short Sample)
        {
            if (!IsRecording)
                return;
            STR.WriteByte((byte)((Sample & 0xFF00) >> 8));
            STR.WriteByte((byte)(Sample & 0xFF));
            NoOfSamples++;
        }
        public void Stop()
        {
            if (IsRecording & STR != null)
            {
                NoOfSamples *= 2;
                SIZE = NoOfSamples + 36;
                byte[] buff_size = new byte[4];
                byte[] buff_NoOfSammples = new byte[4];
                buff_size = BitConverter.GetBytes(SIZE);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_size);
                buff_NoOfSammples = BitConverter.GetBytes(NoOfSamples);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_NoOfSammples);
                IsRecording = false;
                STR.Position = 4;
                STR.Write(buff_size, 0, 4);
                STR.Position = 40;
                STR.Write(buff_NoOfSammples, 0, 4);
                STR.Close();
            }
        }
    }
}
