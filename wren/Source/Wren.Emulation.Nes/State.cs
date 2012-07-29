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
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Wren.Emulation.Nes;
namespace AHD.MyNes.Nes
{
    /*
     * Rewritten by Ala Hadid.
     * 100% works :)
     */
    /// <summary>
    /// The state saver / loader
    /// </summary>
    public class State
    {
        NesEmulator _Nes;
        StateHolder _StateHolder = new StateHolder();
        /// <summary>
        /// The state saver / loader
        /// </summary>
        /// <param name="NesEmu">The current system you want to save / load state from / into</param>
        public State(NesEmulator NesEmu)
        {
            _Nes = NesEmu;
        }
        public void SaveState(string FilePath)
        {
            try
            {
                _Nes.isPaused = true;
                while (!_Nes.my6502.PauseDone)
                { }
                _StateHolder.LoadNesData(_Nes);
                FileStream fs = new FileStream(FilePath, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, _StateHolder);
                fs.Close();
                _Nes.isPaused = false;
            }
            catch { }
        }
        public void LoadState(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    _Nes.isPaused = true;
                    while (!_Nes.my6502.PauseDone)
                    { }
                    FileStream fs = new FileStream(FilePath, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    _StateHolder = (StateHolder)formatter.Deserialize(fs);
                    fs.Close();
                    _StateHolder.ApplyDataToNes(_Nes);
                    _Nes.isPaused = false;
                }
            }
            catch { };
        }
    }
    [Serializable()]
    class StateHolder
    {
        //CPU
        byte a_register;
        byte x_index_register;
        byte y_index_register;
        byte sp_register;
        ushort pc_register;
        byte carry_flag;
        byte zero_flag;
        byte interrupt_flag;
        byte decimal_flag;
        byte brk_flag;
        byte overflow_flag;
        byte sign_flag;
        uint tick_count;
        uint total_cycles = 0;
        byte currentOpcode;
        ushort previousPC;
        //MEMORY
        byte[][] scratchRam;
        byte[] saveRam;
        int JoyData1 = 0;
        int JoyData2 = 0;
        byte JoyStrobe = 0;
        bool isSaveRAMReadonly;
        //CART        
        byte[][] chr_rom;
        MIRRORING mirroring;
        bool save_ram_present;
        bool is_vram;
        uint mirroringBase;
        //MAPPERS
        uint[] current_prg_rom_page;
        uint[] current_chr_rom_page;
        //PPU        
        bool executeNMIonVBlank;
        byte ppuMaster;
        int spriteSize;
        int backgroundAddress;
        int spriteAddress;
        int ppuAddressIncrement;
        int nameTableAddress;
        bool monochromeDisplay;
        bool noBackgroundClipping;
        bool noSpriteClipping;
        bool backgroundVisible;
        bool spritesVisible;
        int ppuColor;
        byte sprite0Hit;
        int vramReadWriteAddress;
        int prev_vramReadWriteAddress;
        byte vramHiLoToggle;
        byte vramReadBuffer;
        byte scrollV;
        byte scrollH;
        int currentScanline;
        byte[] nameTables;
        byte[] spriteRam;
        uint spriteRamAddress;
        int spritesCrossed;
        int frameCounter;
        //APU
        int FrameCounter = 0;
        bool _PAL;
        bool _FrameIRQ;
        //MAPPER 1
        int mapper1_register8000BitPosition;
        int mapper1_registerA000BitPosition;
        int mapper1_registerC000BitPosition;
        int mapper1_registerE000BitPosition;
        int mapper1_register8000Value;
        int mapper1_registerA000Value;
        int mapper1_registerC000Value;
        int mapper1_registerE000Value;
        byte mapper1_mirroringFlag;
        byte mapper1_onePageMirroring;
        byte mapper1_prgSwitchingArea;
        byte mapper1_prgSwitchingSize;
        byte mapper1_vromSwitchingSize;
        //MAPPER 4
        int mapper4_commandNumber;
        int mapper4_prgAddressSelect;
        int mapper4_chrAddressSelect;
        bool mapper4_timer_irq_enabled;
        uint mapper4_timer_irq_count;
        uint mapper4_timer_irq_reload;
        //MAPPER 5
        byte mapper5_prgBankSize;
        byte mapper5_chrBankSize;
        int mapper5_scanlineSplit;
        bool mapper5_splitIrqEnabled;
        //MAPPER 9
        byte mapper9_latch1;
        byte mapper9_latch2;
        int mapper9_latch1data1;
        int mapper9_latch1data2;
        int mapper9_latch2data1;
        int mapper9_latch2data2;
        //MAPPER 10
        byte mapper10_latch1;
        byte mapper10_latch2;
        int mapper10_latch1data1;
        int mapper10_latch1data2;
        int mapper10_latch2data1;
        int mapper10_latch2data2;
        //MAPPER 16
        ushort timer_irq_counter_16 = 0;
        ushort timer_irq_Latch_16 = 0;
        bool timer_irq_enabled;
        //MAPPER 18
        ushort Mapper18_Timer = 0;
        ushort Mapper18_latch = 0;
        byte mapper18_control = 0;
        int Mapper18_IRQWidth = 0;
        bool Mapper18_timer_irq_enabled;
        byte[] Mapper18_x = new byte[22];
        //MAPPER 225
        byte Mapper225_reg0 = 0xF;
        byte Mapper225_reg1 = 0xF;
        byte Mapper225_reg2 = 0xF;
        byte Mapper225_reg3 = 0xF;
        //MAPPER 32
        int mapper32SwitchingMode = 0;
        //MAPPER 41
        byte Mapper41_CHR_Low = 0;
        byte Mapper41_CHR_High = 0;
        //MAPPER 64
        byte mapper64_commandNumber;
        byte mapper64_prgAddressSelect;
        byte mapper64_chrAddressSelect;
        public void LoadNesData(NesEmulator _Nes)
        {
            //CPU
            a_register = _Nes.my6502.a_register;
            x_index_register = _Nes.my6502.x_index_register;
            y_index_register = _Nes.my6502.y_index_register;
            sp_register = _Nes.my6502.sp_register;
            pc_register = _Nes.my6502.pc_register;
            carry_flag = _Nes.my6502.carry_flag;
            zero_flag = _Nes.my6502.zero_flag;
            interrupt_flag = _Nes.my6502.interrupt_flag;
            decimal_flag = _Nes.my6502.decimal_flag;
            brk_flag = _Nes.my6502.brk_flag;
            overflow_flag = _Nes.my6502.overflow_flag;
            sign_flag = _Nes.my6502.sign_flag;
            tick_count = _Nes.my6502.tick_count;
            total_cycles = _Nes.my6502.total_cycles;
            currentOpcode = _Nes.my6502.currentOpcode;
            previousPC = _Nes.my6502.previousPC;
            //MEMORY
            scratchRam = _Nes.scratchRam;
            saveRam = _Nes.saveRam;
            JoyData1 = _Nes.JoyData1;
            JoyData2 = _Nes.JoyData2;
            JoyStrobe = _Nes.JoyStrobe;
            isSaveRAMReadonly = _Nes.isSaveRAMReadonly;
            //CART
            /*
             * We must save the chr_rom ONLY if the cart comes
             * with 0 chr_pages.
             */
            if (_Nes.myCartridge.chr_rom_pages == 0)
                chr_rom = _Nes.myCartridge.chr_rom;
            mirroring = _Nes.myCartridge.mirroring;
            save_ram_present = _Nes.myCartridge.save_ram_present;
            is_vram = _Nes.myCartridge.is_vram;
            mirroringBase = _Nes.myCartridge.mirroringBase;
            //MAPPERS
            current_prg_rom_page = _Nes.myMapper.current_prg_rom_page;
            current_chr_rom_page = _Nes.myMapper.current_chr_rom_page;
            //PPU    
            executeNMIonVBlank = _Nes.myPPU.executeNMIonVBlank;
            ppuMaster = _Nes.myPPU.ppuMaster;
            spriteSize = _Nes.myPPU.spriteSize;
            backgroundAddress = _Nes.myPPU.backgroundAddress;
            spriteAddress = _Nes.myPPU.spriteAddress;
            ppuAddressIncrement = _Nes.myPPU.ppuAddressIncrement;
            nameTableAddress = _Nes.myPPU.nameTableAddress;
            monochromeDisplay = _Nes.myPPU.monochromeDisplay;
            noBackgroundClipping = _Nes.myPPU.noBackgroundClipping;
            noSpriteClipping = _Nes.myPPU.noSpriteClipping;
            backgroundVisible = _Nes.myPPU.backgroundVisible;
            spritesVisible = _Nes.myPPU.spritesVisible;
            ppuColor = _Nes.myPPU.ppuColor;
            sprite0Hit = _Nes.myPPU.sprite0Hit;
            vramReadWriteAddress = _Nes.myPPU.vramReadWriteAddress;
            prev_vramReadWriteAddress = _Nes.myPPU.prev_vramReadWriteAddress;
            vramHiLoToggle = _Nes.myPPU.vramHiLoToggle;
            vramReadBuffer = _Nes.myPPU.vramReadBuffer;
            scrollV = _Nes.myPPU.scrollV;
            scrollH = _Nes.myPPU.scrollH;
            currentScanline = _Nes.myPPU.currentScanline;
            nameTables = _Nes.myPPU.nameTables;
            spriteRam = _Nes.myPPU.spriteRam;
            spriteRamAddress = _Nes.myPPU.spriteRamAddress;
            spritesCrossed = _Nes.myPPU.spritesCrossed;
            frameCounter = _Nes.frameCounter;
            //APU
            FrameCounter = _Nes.myAPU.FrameCounter;
            _PAL = _Nes.myAPU.PAL;
            _FrameIRQ = _Nes.myAPU.FrameIRQ;
            //MAPPERS
            //MAPPER 1
            if (_Nes.myCartridge.mapper == 1)
            {
                Mapper1 map1 = (Mapper1)_Nes.myMapper.CurrentMapper;
                mapper1_register8000BitPosition = map1.mapper1_register8000BitPosition;
                mapper1_registerA000BitPosition = map1.mapper1_registerA000BitPosition;
                mapper1_registerC000BitPosition = map1.mapper1_registerC000BitPosition;
                mapper1_registerE000BitPosition = map1.mapper1_registerE000BitPosition;
                mapper1_register8000Value = map1.mapper1_register8000Value;
                mapper1_registerA000Value = map1.mapper1_registerA000Value;
                mapper1_registerC000Value = map1.mapper1_registerC000Value;
                mapper1_registerE000Value = map1.mapper1_registerE000Value;
                mapper1_mirroringFlag = map1.mapper1_mirroringFlag;
                mapper1_onePageMirroring = map1.mapper1_onePageMirroring;
                mapper1_prgSwitchingArea = map1.mapper1_prgSwitchingArea;
                mapper1_prgSwitchingSize = map1.mapper1_prgSwitchingSize;
                mapper1_vromSwitchingSize = map1.mapper1_vromSwitchingSize;
            }
            //MAPPER 4
            if (_Nes.myCartridge.mapper == 4)
            {
                Mapper4 map4 = (Mapper4)_Nes.myMapper.CurrentMapper;
                mapper4_commandNumber = map4.mapper4_commandNumber;
                mapper4_prgAddressSelect = map4.mapper4_prgAddressSelect;
                mapper4_chrAddressSelect = map4.mapper4_chrAddressSelect;
                mapper4_timer_irq_enabled = map4.timer_irq_enabled;
                mapper4_timer_irq_count = map4.timer_irq_count;
                mapper4_timer_irq_reload = map4.timer_irq_reload;
            }
            //MAPPER 5
            if (_Nes.myCartridge.mapper == 5)
            {
                Mapper5 map5 = (Mapper5)_Nes.myMapper.CurrentMapper;
                mapper5_prgBankSize = map5.mapper5_prgBankSize;
                mapper5_chrBankSize = map5.mapper5_chrBankSize;
                mapper5_scanlineSplit = map5.mapper5_scanlineSplit;
                mapper5_splitIrqEnabled = map5.mapper5_splitIrqEnabled;
            }
            //MAPPER 9
            if (_Nes.myCartridge.mapper == 9)
            {
                Mapper9 map9 = (Mapper9)_Nes.myMapper.CurrentMapper;
                mapper9_latch1 = map9.latch1;
                mapper9_latch2 = map9.latch2;
                mapper9_latch1data1 = map9.latch1data1;
                mapper9_latch1data2 = map9.latch1data2;
                mapper9_latch2data1 = map9.latch2data1;
                mapper9_latch2data2 = map9.latch2data2;
            }
            //MAPPER 10
            if (_Nes.myCartridge.mapper == 10)
            {
                Mapper10 map10 = (Mapper10)_Nes.myMapper.CurrentMapper;
                mapper10_latch1 = map10.latch1;
                mapper10_latch2 = map10.latch2;
                mapper10_latch1data1 = map10.latch1data1;
                mapper10_latch1data2 = map10.latch1data2;
                mapper10_latch2data1 = map10.latch2data1;
                mapper10_latch2data2 = map10.latch2data2;
            }
            //MAPPER 16
            if (_Nes.myCartridge.mapper == 16)
            {
                Mapper16 map16 = (Mapper16)_Nes.myMapper.CurrentMapper;
                timer_irq_counter_16 = map16.timer_irq_counter_16;
                timer_irq_Latch_16 = map16.timer_irq_Latch_16;
                timer_irq_enabled = map16.timer_irq_enabled;
            }
            //MAPPER 18
            if (_Nes.myCartridge.mapper == 18)
            {
                Mapper18 map18 = (Mapper18)_Nes.myMapper.CurrentMapper;
                Mapper18_Timer = map18.Mapper18_Timer;
                Mapper18_latch = map18.Mapper18_latch;
                mapper18_control = map18.mapper18_control;
                Mapper18_IRQWidth = map18.Mapper18_IRQWidth;
                Mapper18_timer_irq_enabled = map18.timer_irq_enabled;
                Mapper18_x = map18.x;
            }
            //MAPPER 225
            if (_Nes.myCartridge.mapper == 225)
            {
                Mapper225 map225 = (Mapper225)_Nes.myMapper.CurrentMapper;
                Mapper225_reg0 = map225.Mapper225_reg0;
                Mapper225_reg1 = map225.Mapper225_reg1;
                Mapper225_reg2 = map225.Mapper225_reg2;
                Mapper225_reg3 = map225.Mapper225_reg3;
            }
            //MAPPER 32
            if (_Nes.myCartridge.mapper == 32)
            {
                Mapper32 map32 = (Mapper32)_Nes.myMapper.CurrentMapper;
                mapper32SwitchingMode = map32.mapper32SwitchingMode;
            }
            //MAPPER 41
            if (_Nes.myCartridge.mapper == 41)
            {
                Mapper41 map41 = (Mapper41)_Nes.myMapper.CurrentMapper;
                Mapper41_CHR_Low = map41.Mapper41_CHR_Low;
                Mapper41_CHR_High = map41.Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_Nes.myCartridge.mapper == 64)
            {
                Mapper64 map64 = (Mapper64)_Nes.myMapper.CurrentMapper;
                mapper64_commandNumber = map64.mapper64_commandNumber;
                mapper64_prgAddressSelect = map64.mapper64_prgAddressSelect;
                mapper64_chrAddressSelect = map64.mapper64_chrAddressSelect;
            }
        }
        public void ApplyDataToNes(NesEmulator _Nes)
        {
            //CPU
            _Nes.my6502.a_register = a_register;
            _Nes.my6502.x_index_register = x_index_register;
            _Nes.my6502.y_index_register = y_index_register;
            _Nes.my6502.sp_register = sp_register;
            _Nes.my6502.pc_register = pc_register;
            _Nes.my6502.carry_flag = carry_flag;
            _Nes.my6502.zero_flag = zero_flag;
            _Nes.my6502.interrupt_flag = interrupt_flag;
            _Nes.my6502.decimal_flag = decimal_flag;
            _Nes.my6502.brk_flag = brk_flag;
            _Nes.my6502.overflow_flag = overflow_flag;
            _Nes.my6502.sign_flag = sign_flag;
            _Nes.my6502.tick_count = tick_count;
            _Nes.my6502.total_cycles = total_cycles;
            _Nes.my6502.currentOpcode = currentOpcode;
            _Nes.my6502.previousPC = previousPC;
            //MEMORY
            _Nes.scratchRam = scratchRam;
            _Nes.saveRam = saveRam;
            _Nes.JoyData1 = JoyData1;
            _Nes.JoyData2 = JoyData2;
            _Nes.JoyStrobe = JoyStrobe;
            _Nes.isSaveRAMReadonly = isSaveRAMReadonly;
            //CART
            if (_Nes.myCartridge.chr_rom_pages == 0)
                _Nes.myCartridge.chr_rom = chr_rom;
            _Nes.myCartridge.mirroring = mirroring;
            _Nes.myCartridge.save_ram_present = save_ram_present;
            _Nes.myCartridge.is_vram = is_vram;
            _Nes.myCartridge.mirroringBase = mirroringBase;
            //MAPPERS
            _Nes.myMapper.current_prg_rom_page = current_prg_rom_page;
            _Nes.myMapper.current_chr_rom_page = current_chr_rom_page;
            //PPU
            _Nes.myPPU.executeNMIonVBlank = executeNMIonVBlank;
            _Nes.myPPU.ppuMaster = ppuMaster;
            _Nes.myPPU.spriteSize = spriteSize;
            _Nes.myPPU.backgroundAddress = backgroundAddress;
            _Nes.myPPU.spriteAddress = spriteAddress;
            _Nes.myPPU.ppuAddressIncrement = ppuAddressIncrement;
            _Nes.myPPU.nameTableAddress = nameTableAddress;
            _Nes.myPPU.monochromeDisplay = monochromeDisplay;
            _Nes.myPPU.noBackgroundClipping = noBackgroundClipping;
            _Nes.myPPU.noSpriteClipping = noSpriteClipping;
            _Nes.myPPU.backgroundVisible = backgroundVisible;
            _Nes.myPPU.spritesVisible = spritesVisible;
            _Nes.myPPU.ppuColor = ppuColor;
            _Nes.myPPU.sprite0Hit = sprite0Hit;
            _Nes.myPPU.vramReadWriteAddress = vramReadWriteAddress;
            _Nes.myPPU.prev_vramReadWriteAddress = prev_vramReadWriteAddress;
            _Nes.myPPU.vramHiLoToggle = vramHiLoToggle;
            _Nes.myPPU.vramReadBuffer = vramReadBuffer;
            _Nes.myPPU.scrollV = scrollV;
            _Nes.myPPU.scrollH = scrollH;
            _Nes.myPPU.currentScanline = currentScanline;
            _Nes.myPPU.nameTables = nameTables;
            _Nes.myPPU.spriteRam = spriteRam;
            _Nes.myPPU.spriteRamAddress = spriteRamAddress;
            _Nes.myPPU.spritesCrossed = spritesCrossed;
            _Nes.frameCounter = frameCounter;
            //APU
            _Nes.myAPU.FrameCounter = FrameCounter;
            _Nes.myAPU.PAL = _PAL;
            _Nes.myAPU.FrameIRQ = _FrameIRQ;
            //MAPPER 1
            if (_Nes.myCartridge.mapper == 1)
            {
                Mapper1 map1 = (Mapper1)_Nes.myMapper.CurrentMapper;
                map1.mapper1_register8000BitPosition = mapper1_register8000BitPosition;
                map1.mapper1_registerA000BitPosition = mapper1_registerA000BitPosition;
                map1.mapper1_registerC000BitPosition = mapper1_registerC000BitPosition;
                map1.mapper1_registerE000BitPosition = mapper1_registerE000BitPosition;
                map1.mapper1_register8000Value = mapper1_register8000Value;
                map1.mapper1_registerA000Value = mapper1_registerA000Value;
                map1.mapper1_registerC000Value = mapper1_registerC000Value;
                map1.mapper1_registerE000Value = mapper1_registerE000Value;
                map1.mapper1_mirroringFlag = mapper1_mirroringFlag;
                map1.mapper1_onePageMirroring = mapper1_onePageMirroring;
                map1.mapper1_prgSwitchingArea = mapper1_prgSwitchingArea;
                map1.mapper1_prgSwitchingSize = mapper1_prgSwitchingSize;
                map1.mapper1_vromSwitchingSize = mapper1_vromSwitchingSize;
            }
            //MAPPER 4
            if (_Nes.myCartridge.mapper == 4)
            {
                Mapper4 map4 = (Mapper4)_Nes.myMapper.CurrentMapper;
                map4.mapper4_commandNumber = mapper4_commandNumber;
                map4.mapper4_prgAddressSelect = mapper4_prgAddressSelect;
                map4.mapper4_chrAddressSelect = mapper4_chrAddressSelect;
                map4.timer_irq_enabled = mapper4_timer_irq_enabled;
                map4.timer_irq_count = mapper4_timer_irq_count;
                map4.timer_irq_reload = mapper4_timer_irq_reload;
            }
            //MAPPER 5
            if (_Nes.myCartridge.mapper == 5)
            {
                Mapper5 map5 = (Mapper5)_Nes.myMapper.CurrentMapper;
                map5.mapper5_prgBankSize = mapper5_prgBankSize;
                map5.mapper5_chrBankSize = mapper5_chrBankSize;
                map5.mapper5_scanlineSplit = mapper5_scanlineSplit;
                map5.mapper5_splitIrqEnabled = mapper5_splitIrqEnabled;
            }
            //MAPPER 9
            if (_Nes.myCartridge.mapper == 9)
            {
                Mapper9 map9 = (Mapper9)_Nes.myMapper.CurrentMapper;
                map9.latch1 = mapper9_latch1;
                map9.latch2 = mapper9_latch2;
                map9.latch1data1 = mapper9_latch1data1;
                map9.latch1data2 = mapper9_latch1data2;
                map9.latch2data1 = mapper9_latch2data1;
                map9.latch2data2 = mapper9_latch2data2;
            }
            //MAPPER 10
            if (_Nes.myCartridge.mapper == 10)
            {
                Mapper10 map10 = (Mapper10)_Nes.myMapper.CurrentMapper;
                map10.latch1 = mapper10_latch1;
                map10.latch2 = mapper10_latch2;
                map10.latch1data1 = mapper10_latch1data1;
                map10.latch1data2 = mapper10_latch1data2;
                map10.latch2data1 = mapper10_latch2data1;
                map10.latch2data2 = mapper10_latch2data2;
            }
            //MAPPER 16
            if (_Nes.myCartridge.mapper == 16)
            {
                Mapper16 map16 = (Mapper16)_Nes.myMapper.CurrentMapper;
                map16.timer_irq_counter_16 = timer_irq_counter_16;
                map16.timer_irq_Latch_16 = timer_irq_Latch_16;
                map16.timer_irq_enabled = timer_irq_enabled;
            }
            //MAPPER 18
            if (_Nes.myCartridge.mapper == 18)
            {
                Mapper18 map18 = (Mapper18)_Nes.myMapper.CurrentMapper;
                map18.Mapper18_Timer = Mapper18_Timer;
                map18.Mapper18_latch = Mapper18_latch;
                map18.mapper18_control = mapper18_control;
                map18.Mapper18_IRQWidth = Mapper18_IRQWidth;
                map18.timer_irq_enabled = Mapper18_timer_irq_enabled;
                map18.x = Mapper18_x;
            }
            //MAPPER 225
            if (_Nes.myCartridge.mapper == 225)
            {
                Mapper225 map225 = (Mapper225)_Nes.myMapper.CurrentMapper;
                map225.Mapper225_reg0 = Mapper225_reg0;
                map225.Mapper225_reg1 = Mapper225_reg1;
                map225.Mapper225_reg2 = Mapper225_reg2;
                map225.Mapper225_reg3 = Mapper225_reg3;
            }
            //MAPPER 32
            if (_Nes.myCartridge.mapper == 32)
            {
                Mapper32 map32 = (Mapper32)_Nes.myMapper.CurrentMapper;
                map32.mapper32SwitchingMode = mapper32SwitchingMode;
            }
            //MAPPER 41
            if (_Nes.myCartridge.mapper == 41)
            {
                Mapper41 map41 = (Mapper41)_Nes.myMapper.CurrentMapper;
                map41.Mapper41_CHR_Low = Mapper41_CHR_Low;
                map41.Mapper41_CHR_High = Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_Nes.myCartridge.mapper == 64)
            {
                Mapper64 map64 = (Mapper64)_Nes.myMapper.CurrentMapper;
                map64.mapper64_commandNumber = mapper64_commandNumber;
                map64.mapper64_prgAddressSelect = mapper64_prgAddressSelect;
                map64.mapper64_chrAddressSelect = mapper64_chrAddressSelect;
            }
        }
    }
}
