using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AHD.MyNes.Nes;

namespace Wren.Emulation.Nes
{
    public class SystemOrchestration
    {
        PictureProcessingUnit _ppu;
        NesEmulator _emulator;
        AudioProcessingUnit _apu;
        Cpu6502 _cpu;
        SystemBus _systemBus;

        Int32 frameCount = 0;

        UInt32 extraCycles = 0;

        public SystemOrchestration(AudioProcessingUnit apu,
                            PictureProcessingUnit ppu,
                            SystemBus bus,
                            Cpu6502 cpu,
                            NesEmulator emulator)
        {
            _apu = apu;
            _ppu = ppu;
            _cpu = cpu;
            _systemBus = bus;
            _emulator = emulator;
        }

        public void RunFrame()
        {
            if (frameCount == 0)
            {
                _cpu.pc_register = _systemBus.ReadMemory16(0xFFFC);
            }

            for (int scanline = 0; scanline <= _ppu.Scanlinesperframe; scanline++)
            {
                RunScanline(scanline);
            }

            if (_emulator.SoundEnabled)
            {
                _apu.RenderFrame();
            }

            _emulator.frameCounter++;
            _ppu.sprite0Hit = 0;

            frameCount++;
        }

        public virtual void RunScanline(int scanline)
        {
            _ppu.currentScanline = scanline;

            UInt32 ticks = 0;
            UInt32 requestedTicks = _emulator.Ticks_Per_Scanline - extraCycles;

            while (ticks < requestedTicks)
            {
                ticks += _cpu.RunNextInstruction();
                _apu.Play();
            }

            extraCycles = ticks - requestedTicks;

            if ((scanline == _ppu.ScanlinesOfVBLANK) && (_ppu.executeNMIonVBlank))
            {
                _cpu.Push16(_cpu.pc_register);
                _cpu.PushStatus();
                _cpu.pc_register = _systemBus.ReadMemory16(0xFFFA);
            }

            if (scanline < 240)
            {
                _ppu.RenderScanline(scanline);
            }
        }
    }
}
