using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SlimDX;
using SlimDX.DirectInput;
using Wren.Emulation.Nes;
using AHD.MyNes.Nes;
using Wren.Core;
using AHD.MyNes.Core;
using Wren.Core.Debugging;

namespace Wren.Emulation.Nes
{
    public class NesEmulator : IEmulator, IDebuggingEmulator
    {
        public IEventAggregator eventAggregator;
        public HashSet<Int32> memoryWatchLocations;

        public int JoyData1 = 0;
        public int JoyData2 = 0;
        public byte JoyStrobe = 0;
        private NesSystem _CurrentSystem = NesSystem.PAL;
        public Joypad _joypad1;
        public Joypad _joypad2;

        public uint Ticks_Per_Scanline = 113;//113 for NTSC, 106 for PAL

        //Devices in the nes
        public Cart myCartridge;// Nes cart
        public Cpu6502 my6502;// CPU
        public Mappers myMapper;//Current mapper
        public PictureProcessingUnit myPPU;//PPU
        public AudioProcessingUnit myAPU;
        public SystemBus systemBus;

        //Fields
        public bool isQuitting;
        public bool hasQuit;
        public bool isDebugging;
        public bool isSaveRAMReadonly;
        public bool isPaused;

        public bool fix_scrolloffset1;
        public bool fix_scrolloffset2;
        public bool fix_scrolloffset3;
        public bool LimitTo50 = false;
        public bool LimitTo60 = false;
        public byte[][] scratchRam;
        public byte[] saveRam;
        public bool SoundEnabled = true;
        public string FileName = "";
        public bool SpeedThrottling = false;

        private Int32 _framesPerSecond;
        public int frameCounter;


        public Joypad Joypad1 { get { return _joypad1; } }
        public Joypad Joypad2 { get { return _joypad2; } }
        
        public NesSystem CurrentSystem
        {
            get
            {
                return _CurrentSystem;
            }
            set
            {
                _CurrentSystem = value;

                switch (value)
                {
                    case NesSystem.NTSC:
                        Ticks_Per_Scanline = 113;
                        _framesPerSecond = 60;
                        myPPU.Scanlinesperframe = 262;
                        //////////////////////////////
                        //This value is important, in doc's, they say
                        //that VBLANK is happens in SL (scan line) # 240
                        //but in real test (with some roms like Castlequest),
                        //this No will make a bad scroll in the game ...
                        //however i test it and 248 (with NTSC) is verygood
                        //for this game and others (Castlequest, Castlevania ..)
                        //remember that the PPU returns VBLANK in SL # 240
                        //in the status register
                        //while the real one happens after that lil' bit.
                        myPPU.ScanlinesOfVBLANK = 248;
                        //////////////////////////////
                        break;
                    case NesSystem.PAL:
                        Ticks_Per_Scanline = 106;
                        _framesPerSecond = 50;
                        myPPU.Scanlinesperframe = 312;
                        myPPU.ScanlinesOfVBLANK = 290;
                        break;
                }
            }
        }
        public NesEmulator()
        {
            memoryWatchLocations = new HashSet<Int32>();
            _joypad1 = new Joypad();
            _joypad2 = new Joypad();
            systemBus = new SystemBus(this);
            myAPU = new AudioProcessingUnit(WrenCore.WindowHandle, this, systemBus);
        }

        public void RestartEngine()
        {
            isSaveRAMReadonly = false;
            isDebugging = false;
            isQuitting = false;
            isPaused = false;
            hasQuit = false;
            //fix_bgchange = false;
            //fix_spritehit = false;
            fix_scrolloffset1 = false;
            fix_scrolloffset2 = false;
            fix_scrolloffset3 = false;
            myPPU.RestartPPU();
            frameCounter = 0;
        }
        /// <summary>
        /// Quit (turn off)
        /// </summary>
        public void QuitEngine()
        {
            while (myAPU.IsRendering)
            { }
            myAPU.Pause();
            myAPU.Shutdown();
            isQuitting = true;
        }
        public void TogglePause()
        {
            if (isPaused)
            {
                isPaused = false;
            }
            else
            {
                isPaused = true;
            }
        }
        SystemOrchestration orchestrator;
 
        //Cart ...
        private bool LoadCart(Stream romData, Stream saveData)
        {
            byte[] nesHeader = new byte[16];
            int i;
            try
            {
                using (var reader = new BinaryReader(romData))
                {
                    reader.Read(nesHeader, 0, 16);
                    int prg_roms = nesHeader[4] * 4;
                    myCartridge.prg_rom_pages = nesHeader[4];
                    myCartridge.prg_rom = new byte[prg_roms][];
                    for (i = 0; i < (prg_roms); i++)
                    {
                        myCartridge.prg_rom[i] = new byte[4096];
                        reader.Read(myCartridge.prg_rom[i], 0, 4096);
                    }
                    int chr_roms = nesHeader[5] * 8;
                    myCartridge.chr_rom_pages = nesHeader[5];
                    if (myCartridge.chr_rom_pages != 0)
                    {
                        myCartridge.chr_rom = new byte[chr_roms][];
                        for (i = 0; i < (chr_roms); i++)
                        {
                            myCartridge.chr_rom[i] = new byte[1024];
                            reader.Read(myCartridge.chr_rom[i], 0, 1024);
                        }
                        myCartridge.is_vram = false;
                    }
                    else
                    {
                        myCartridge.chr_rom = new byte[16][];
                        for (i = 0; i < 16; i++)
                        {
                            myCartridge.chr_rom[i] = new byte[1024];
                        }
                        myCartridge.is_vram = true;
                    }
                    if ((nesHeader[6] & 0x1) == 0x0)
                    {
                        myCartridge.mirroring = MIRRORING.HORIZONTAL;
                    }
                    else
                    {
                        myCartridge.mirroring = MIRRORING.VERTICAL;
                    }

                    if ((nesHeader[6] & 0x2) == 0x0)
                    {
                        myCartridge.save_ram_present = false;
                    }
                    else
                    {
                        myCartridge.save_ram_present = true;
                    }

                    if ((nesHeader[6] & 0x4) == 0x0)
                    {
                        myCartridge.trainer_present = false;
                    }
                    else
                    {
                        myCartridge.trainer_present = true;
                    }

                    if ((nesHeader[6] & 0x8) != 0x0)
                    {
                        myCartridge.mirroring = MIRRORING.FOUR_SCREEN;
                    }

                    if (nesHeader[7] == 0x44)
                    {
                        myCartridge.mapper = (byte)(nesHeader[6] >> 4);
                    }
                    else
                    {
                        myCartridge.mapper = (byte)((nesHeader[6] >> 4) + (nesHeader[7] & 0xf0));
                    }
                    if ((nesHeader[6] == 0x23) && (nesHeader[7] == 0x64))
                        myCartridge.mapper = 2;
                    /*if ((myCartridge.prg_rom[prg_roms - 1][0xfeb] == 'Z') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfec] == 'E') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfed] == 'L') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfee] == 'D') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfef] == 'A'))
                    {
                        fix_bgchange = true;
                    }*/
                    if ((myCartridge.prg_rom[prg_roms - 1][0xfe0] == 'B') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe1] == 'B') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe2] == '4') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe3] == '7') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe4] == '9') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe5] == '5') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe6] == '6') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe7] == '-') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe8] == '1') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfe9] == '5') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfea] == '4') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfeb] == '4') &&
                        (myCartridge.prg_rom[prg_roms - 1][0xfec] == '0'))
                    {
                        fix_scrolloffset1 = true;
                    }
                    if ((myCartridge.prg_rom[0][0x9] == 0xfc) &&
                        (myCartridge.prg_rom[0][0xa] == 0xfc) &&
                        (myCartridge.prg_rom[0][0xb] == 0xfc) &&
                        (myCartridge.prg_rom[0][0xc] == 0x40) &&
                        (myCartridge.prg_rom[0][0xd] == 0x40) &&
                        (myCartridge.prg_rom[0][0xe] == 0x40) &&
                        (myCartridge.prg_rom[0][0xf] == 0x40))
                    {
                        fix_scrolloffset2 = true;
                    }
                    if ((myCartridge.prg_rom[0][0x75] == 0x11) &&
                        (myCartridge.prg_rom[0][0x76] == 0x12) &&
                        (myCartridge.prg_rom[0][0x77] == 0x13) &&
                        (myCartridge.prg_rom[0][0x78] == 0x14) &&
                        (myCartridge.prg_rom[0][0x79] == 0x07) &&
                        (myCartridge.prg_rom[0][0x7a] == 0x03) &&
                        (myCartridge.prg_rom[0][0x7b] == 0x03) &&
                        (myCartridge.prg_rom[0][0x7c] == 0x03) &&
                        (myCartridge.prg_rom[0][0x7d] == 0x03)
                        )
                    {
                        fix_scrolloffset3 = true;
                    }
                    myMapper.SetUpMapperDefaults();
                }
            }
            catch
            {
                return false;
            }

            if (myCartridge.save_ram_present)
            {
                try
                {
                    if (saveData == null)
                        return true;
                    using (var reader = new BinaryReader(saveData))
                    {
                        reader.Read(saveRam, 0, 0x2000);
                    }
                }
                catch
                {
                    //Ignore it, we'll make our own.
                }
            }
            return true;
        }

        public void Initialize(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            myCartridge = new Cart();
            my6502 = new Cpu6502(this, systemBus);
            myMapper = new Mappers(this, ref myCartridge, systemBus);
            myPPU = new PictureProcessingUnit(this, systemBus);
            orchestrator = new SystemOrchestration(myAPU, myPPU, systemBus, my6502, this);

            scratchRam = new byte[4][];
            scratchRam[0] = new byte[0x800];
            scratchRam[1] = new byte[0x800];
            scratchRam[2] = new byte[0x800];
            scratchRam[3] = new byte[0x800];
            saveRam = new byte[0x2000];

            isSaveRAMReadonly = false;
            isDebugging = false;
            isQuitting = false;
            isPaused = false;
            hasQuit = false;
            //fix_bgchange = false;
            //fix_spritehit = false;
            fix_scrolloffset1 = false;
            fix_scrolloffset2 = false;
            fix_scrolloffset3 = false;

            MainCore.Settings = new Settings();
            //Palette
            this.myPPU.LoadPalette(MainCore.Settings.PalettePath);
            //sound
            this.myAPU.DMCEnabled = MainCore.Settings.DMCEnabled;
            this.myAPU.NoiseEnabled = MainCore.Settings.NoiseEnabled;
            this.myAPU.Square1Enabled = MainCore.Settings.SQR1enabled;
            this.myAPU.Square2Enabled = MainCore.Settings.SQR2enabled;
            this.myAPU.TriangleEnabled = MainCore.Settings.TRIenabled;

            //region
            this.CurrentSystem = MainCore.Settings.NesRegion;

            this.SpeedThrottling = MainCore.Settings.SpeedThrottling;

            this.isPaused = false;
        }

        public void GetSpecifications(out int screenWidth, out int screenHeight, out int refreshRate, out Wren.Core.PixelFormats pixelFormat)
        {
            screenHeight = 240;
            screenWidth = 256;
            refreshRate = 60;
            pixelFormat = PixelFormats.Bgr565;
        }

        public void SetRenderingSurface(IntPtr renderingSurface)
        {
            myPPU.SetRenderingSurface(renderingSurface);
        }

        public void LoadRom(Stream romData, Stream saveData)
        {
            LoadCart(romData, saveData);
        }

        public Boolean Run()
        {
            orchestrator.RunFrame();
            return !isQuitting;
        }

        public Boolean IsRomValid(Stream romData)
        {
           CartHeaderReader rom = new CartHeaderReader(romData);

            if (rom.validRom)
            {
                if (rom.SupportedMapper())
                {
                    return true;
                }
            }

            return false;
        }

        public void SetInput(InputState input)
        {
            Joypad.InputState = input;
        }

        public void Quit()
        {
            QuitEngine();
        }

        public void DisableSound()
        {
            SoundEnabled = false;
        }

        public void DumpMemory()
        {
            List<Byte> ram = new List<Byte>();
            ram.AddRange(scratchRam[0]);
            ram.AddRange(scratchRam[1]);
            ram.AddRange(scratchRam[2]);
            ram.AddRange(scratchRam[3]);
            MemoryDumpedEvent mde = new MemoryDumpedEvent(ram.ToArray(), 0x0000);

            eventAggregator.Publish(mde);
        }

        public void SetMemoryWatch8(int memoryAddress)
        {
            memoryWatchLocations.Add(memoryAddress);
        }


        public void SetBreakPoint(Int32 address)
        {
            throw new NotImplementedException();
        }

        public int ReadMemory(int address)
        {
            throw new NotImplementedException();
        }
    }
    public enum NesSystem
    {
        NTSC, PAL
    }
}