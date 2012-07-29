using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.Nes;
using Wren.Core.Debugging;
using Wren.Core.Events;

namespace AHD.MyNes.Nes
{
    public class SystemBus
    {
        NesEmulator _nes;

        public SystemBus(NesEmulator nes)
        {
            _nes = nes;
        }

        //Memory
        public byte ReadMemory8(ushort address)
        {
            byte returnvalue = 0;
            if (address < 0x2000)
            {
                if (address < 0x800)
                {
                    returnvalue = _nes.scratchRam[0][address];
                }
                else if (address < 0x1000)
                {
                    returnvalue = _nes.scratchRam[1][address - 0x800];
                }

                else if (address < 0x1800)
                {
                    returnvalue = _nes.scratchRam[2][address - 0x1000];
                }
                else
                {
                    returnvalue = _nes.scratchRam[3][address - 0x1800];
                }
            }
            else if (address < 0x6000)
            {
                if (address == 0x2002)
                { returnvalue = _nes.myPPU.Status_Register_Read(); }
                else if (address == 0x2004)
                { returnvalue = _nes.myPPU.SpriteRam_IO_Register_Read(); }
                else if (address == 0x2007)
                { returnvalue = _nes.myPPU.VRAM_IO_Register_Read(); }
                else if (address == 0x4015)
                { returnvalue = _nes.myAPU.Read_4015(); }
                else if (address == 0x4016)
                {
                    byte num2 = (byte)(_nes.JoyData1 & 1);
                    _nes.JoyData1 = _nes.JoyData1 >> 1;
                    returnvalue = num2;
                }
                else if (address == 0x4017)
                {
                    byte num3 = (byte)(_nes.JoyData2 & 1);
                    _nes.JoyData2 = _nes.JoyData2 >> 1;
                    returnvalue = num3;
                }
            }
            else if (address < 0x8000)
            {
                returnvalue = _nes.saveRam[address - 0x6000];
                if (_nes.myCartridge.mapper == 5)
                    returnvalue = 1;
            }
            else
            {
                returnvalue = _nes.myMapper.ReadPrgRom(address);
            }
            return returnvalue;
        }
        public byte WriteMemory8(ushort address, byte data)
        {
            if (address < 0x2000)
            {
                if (_nes.memoryWatchLocations.Contains(address))
                    _nes.eventAggregator.Publish(new MemoryValueChangedEvent(address, data));

                if (address < 0x800)
                {
                    _nes.scratchRam[0][address] = data;
                }
                else if (address < 0x1000)
                {
                    _nes.scratchRam[1][address - 0x800] = data;
                }
                else if (address < 0x1800)
                {
                    _nes.scratchRam[2][address - 0x1000] = data;
                }
                else if (address < 0x2000)
                {
                    _nes.scratchRam[3][address - 0x1800] = data;
                }
            }
            else if (address < 0x4000)
            {
                if (address == 0x2000)
                { _nes.myPPU.Control_Register_1_Write(data); }
                else if (address == 0x2001)
                { _nes.myPPU.Control_Register_2_Write(data); }
                else if (address == 0x2003)
                { _nes.myPPU.SpriteRam_Address_Register_Write(data); }
                else if (address == 0x2004)
                { _nes.myPPU.SpriteRam_IO_Register_Write(data); }
                else if (address == 0x2005)
                { _nes.myPPU.VRAM_Address_Register_1_Write(data); }
                else if (address == 0x2006)
                { _nes.myPPU.VRAM_Address_Register_2_Write(data); }
                else if (address == 0x2007)
                { _nes.myPPU.VRAM_IO_Register_Write(data); }
            }
            else if (address < 0x6000)
            {
                //APU registers
                if (address == 0x4000)
                { _nes.myAPU.Write_4000(data); }
                else if (address == 0x4001)
                { _nes.myAPU.Write_4001(data); }
                else if (address == 0x4002)
                { _nes.myAPU.Write_4002(data); }
                else if (address == 0x4003)
                { _nes.myAPU.Write_4003(data); }
                else if (address == 0x4004)
                { _nes.myAPU.Write_4004(data); }
                else if (address == 0x4005)
                { _nes.myAPU.Write_4005(data); }
                else if (address == 0x4006)
                { _nes.myAPU.Write_4006(data); }
                else if (address == 0x4007)
                { _nes.myAPU.Write_4007(data); }
                else if (address == 0x4008)
                { _nes.myAPU.Write_4008(data); }
                else if (address == 0x400A)
                { _nes.myAPU.Write_400A(data); }
                else if (address == 0x400B)
                { _nes.myAPU.Write_400B(data); }
                else if (address == 0x400C)
                { _nes.myAPU.Write_400C(data); }
                else if (address == 0x400E)
                { _nes.myAPU.Write_400E(data); }
                else if (address == 0x400F)
                { _nes.myAPU.Write_400F(data); }
                else if (address == 0x4010)
                { _nes.myAPU.Write_4010(data); }
                else if (address == 0x4011)
                { _nes.myAPU.Write_4011(data); }
                else if (address == 0x4012)
                { _nes.myAPU.Write_4012(data); }
                else if (address == 0x4013)
                { _nes.myAPU.Write_4013(data); }
                else if (address == 0x4014)
                { _nes.myPPU.SpriteRam_DMA_Begin(data); }
                else if (address == 0x4015)
                { _nes.myAPU.Write_4015(data); }
                else if (address == 0x4016)
                {
                    if ((this._nes.JoyStrobe == 1) && ((data & 1) == 0))
                    {
                        this._nes.JoyData1 = _nes._joypad1.GetJoyData(Joypad.InputState, 0) | 0x100;
                        this._nes.JoyData2 = _nes._joypad2.GetJoyData(Joypad.InputState, 1) | 0x200;
                    }
                    this._nes.JoyStrobe = (byte)(data & 1);
                }
                else if (address == 0x4017)
                { _nes.myAPU.Write_4017(data); }

                if (_nes.myCartridge.mapper == 5)
                    _nes.myMapper.WritePrgRom(address, data);
            }
            else if (address < 0x8000)
            {
                if (!_nes.isSaveRAMReadonly)
                { _nes.saveRam[address - 0x6000] = data; }
                if (_nes.myCartridge.mapper == 34 | _nes.myCartridge.mapper == 16)
                { _nes.myMapper.WritePrgRom(address, data); }
            }
            else
            {
                _nes.myMapper.WritePrgRom(address, data);
            }
            return 1;
        }
        public ushort ReadMemory16(ushort address)
        {
            byte data_1 = 0;
            byte data_2 = 0;
            if (address < 0x2000)
            {
                if (address < 0x800)
                {
                    data_1 = _nes.scratchRam[0][address];
                    data_2 = _nes.scratchRam[0][address + 1];
                }
                else if (address < 0x1000)
                {
                    data_1 = _nes.scratchRam[1][address - 0x800];
                    data_2 = _nes.scratchRam[1][address - 0x800 + 1];
                }

                else if (address < 0x1800)
                {
                    data_1 = _nes.scratchRam[2][address - 0x1000];
                    data_2 = _nes.scratchRam[2][address - 0x1000 + 1];
                }
                else
                {
                    data_1 = _nes.scratchRam[3][address - 0x1800];
                    data_2 = _nes.scratchRam[3][address - 0x1800 + 1];
                }
            }
            else if (address < 0x8000)
            {
                data_1 = _nes.saveRam[address - 0x6000];
                data_2 = _nes.saveRam[address - 0x6000 + 1];
            }
            else
            {
                data_1 = _nes.myMapper.ReadPrgRom(address);
                data_2 = _nes.myMapper.ReadPrgRom((ushort)(address + 1));
            }
            ushort data = (ushort)((data_2 << 8) + data_1);
            return data;
        }
    }
}
