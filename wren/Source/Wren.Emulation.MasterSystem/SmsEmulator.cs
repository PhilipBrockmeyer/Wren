using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wren.Core;
using Wren.Core.Events;

namespace Wren.Emulation.MasterSystem
{
    public class SmsEmulator : IEmulator, IDebuggingEmulator, IStatefulEmulator
    {
        Boolean _isRunning;
        ComponentManager _componentManager;
        BreakpointHandler _breakpointHandler;
        public ISystemBus _systemBus;
        public Z80Cpu _cpu;
        public Cartridge _cart;
        public ReferenceImplementation _ri;
        public VideoDisplayProcessor _vdp;
        public Ram _ram;
        GamepadPorts _gamepads;
        IEventAggregator _eventAggregator;
        IntPtr _screenBuffer;

        public SmsEmulator()
        {
            _cart = new Cartridge();
            _breakpointHandler = new BreakpointHandler(_cart);
            _cpu = new Z80Cpu(_breakpointHandler);
            _cpu.InstructionRan += new System.EventHandler<InstructionAdvice.CpuEventArgs>(_cpu_InstructionRan);
            _cpu.IsRecentHistoryEnabled = false;

            _ri = new ReferenceImplementation();            
            _ram = new Ram();
            _vdp = new VideoDisplayProcessor();
            _gamepads = new GamepadPorts();

            IInteruptManager interuptManager = new InteruptManager();
            IPortManager portManager = new PortManager();
            IAddressManager addressManager = new AddressManager(new CacheManager(_cpu.Data));

            _systemBus = new SystemBus(interuptManager, addressManager, portManager);
            _componentManager = new ComponentManager(interuptManager, addressManager, portManager);
        }

        void _cpu_InstructionRan(object sender, InstructionAdvice.CpuEventArgs e)
        {
        }

        public void Run(Int32 cycles)
        {
            try
            {
                _componentManager.RunFrame(cycles, _systemBus);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StartDegbugging()
        {
            _componentManager.DetachComponent(_cpu);
            _cpu.IsDebugModeEnabled = true;
            _componentManager.AttachComponent(_cpu);
            _componentManager.InitializeComponents(_systemBus);
        }

        public void StopDebugging()
        {
            _componentManager.DetachComponent(_cpu);
            _cpu.IsDebugModeEnabled = false;
            _componentManager.AttachComponent(_cpu);
            _componentManager.InitializeComponents(_systemBus);
        }

        public void Initialize(IEventAggregator eventAggregator)
        {
            _isRunning = true;
            _eventAggregator = eventAggregator;

            if (_systemBus is IDebuggingSystemBus)
            {
                ((IDebuggingSystemBus)_systemBus).ValueChanged += new System.EventHandler<ValueChangedEventArgs>(
                    (sender, e) => eventAggregator.Publish(new MemoryValueChangedEvent(e.Address, e.Value)));
            }

            _breakpointHandler.BreakpointHit +=new System.EventHandler<BreakpointHitEventArgs>(
                (sender, e) => eventAggregator.Publish(new BreakpointHitEvent(e.Address)));           
           
            _componentManager.AttachComponent(_cpu);
            _componentManager.AttachComponent(_ram);
            _componentManager.AttachComponent(_vdp);
            _componentManager.AttachComponent(new UnknownPorts());
            _componentManager.AttachComponent(_gamepads);
            _componentManager.AttachComponent(new SN76489());
            _componentManager.AttachComponent(_cart);
            _componentManager.AttachComponent(_breakpointHandler);
            _componentManager.AttachComponent(new YM2413());
            _componentManager.InitializeComponents(_systemBus);
        }

        void SmsEmulator_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void GetSpecifications(out int screenWidth, out int screenHeight, out int refreshRate, out PixelFormats pixelFormat)
        {
            screenWidth = 256;
            screenHeight = 192;
            refreshRate = 60;
            pixelFormat = PixelFormats.Bgr565;
        }

        public void SetRenderingSurface(IntPtr renderingSurface)
        {
            _vdp.SetScreeenBuffer(renderingSurface);
        }

        public void SetInput(InputState input)
        {
            _gamepads.SetInputState(input);
        }

        public void LoadRom(Stream romData, Stream saveData)
        {
            Byte[] bin = new Byte[romData.Length];
            romData.Read(bin, 0, (Int32)romData.Length);
            _cart.LoadRomData(bin);
        }

        public Boolean Run()
        {
            if (!_isRunning)
                return false;

            for (Int32 i = 0; i < 224; i++)
            {
                Run(3580000 / 60 / 224);
            }

            return true;
        }

        public Boolean IsRomValid(Stream romData)
        {
            return true;
        }

        public void DisableSound()
        {
        }

        public void Quit()
        {
            _isRunning = false;
        }

        public void DumpMemory()
        {
            _eventAggregator.Publish(new MemoryDumpedEvent(_ram._memory.Take(0x2000).ToArray(), 0xC000));
        }

        public void SetMemoryWatch8(Int32 memoryAddress)
        {
            if (_systemBus is IDebuggingSystemBus)
            {
                ((IDebuggingSystemBus)_systemBus).AddMemoryWatch(memoryAddress);
            }
        }

        public void SetBreakPoint(Int32 address)
        {
            _breakpointHandler.AddBreakpoint(address, 1 /* Instruction Size */);
        }

        public Int32 ReadMemory(Int32 address)
        {
            return _systemBus.ReadByte(address);
        }

        public void SaveState(BinaryWriter writer)
        {
            _componentManager.SaveComponentState(writer);
        }

        public void LoadState(BinaryReader reader)
        {
            _componentManager.LoadComponentState(reader);
        }
    }
}
