using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Events;

namespace Wren.Core
{
    public class EmulationRunner : IEmulationRunner
    {
        public Guid InstanceId { get; private set; }

        IFrameRateTimerFactory _frameRateTimerFactory;
        IEmulatorRegistry _emulatorRegistry;
        IEmulator _emulator;
        IntPtr _handle;
        IInputSourceAssembler _inputPipeline;
        RenderingSourceFactory _renderingSourceFactory;
        MessageBus _bus;

        public IEmulator Emulator { get { return _emulator; } }
        
        public EmulationRunner(IFrameRateTimerFactory frameRateTimerFactory, IInputSourceAssembler inputSourceAssembler, IEmulatorRegistry emulatorRegistry, MessageBus messageBus, RenderingSourceFactory renderingSourceFactory)
        {
            InstanceId = Guid.NewGuid();

            _emulatorRegistry = emulatorRegistry;
            _handle = WrenCore.WindowHandle;
            _inputPipeline = inputSourceAssembler;
            _frameRateTimerFactory = frameRateTimerFactory;
            _bus = messageBus;
            _renderingSourceFactory = renderingSourceFactory;
        }

        public void SendCommand(IEmulatorCommand command)
        {
            _bus.SendCommand(command);
        }

        public void Start(EmulationContext context, IEventAggregator eventAggregator, EmulationMode mode)
        {
            _emulator = _emulatorRegistry.GetEmulator(context.EmulatedSystem, _handle);

            IRomSource loader = null;

            if (context.Game.RomPath.ToLower().EndsWith(".zip"))
            {
                loader = new ZipRomSource(context.Game.RomPath);
            }
            else
            {
                loader = new FileRomSource(context.Game.RomPath);
            }

            using (var romData = loader.GetRomData())
            {
                if (!_emulator.IsRomValid(romData))
                    return;

                romData.Seek(0, System.IO.SeekOrigin.Begin);

                _emulator.LoadRom(romData, null);
            }

            eventAggregator.Publish(new EmulatorStartingEvent(InstanceId, this, context.Game, mode));

            while (_bus.HasMessages)
            {
                _bus.GetCommand().Execute(_emulator);
            }

            _emulator.Initialize(eventAggregator);

            int pixelWidth, pixelHeight;
            Wren.Core.PixelFormats requestedPixelFormat;
            Int32 framePerSecond;

            // assemble rendering pipeline
            _emulator.GetSpecifications(out pixelWidth, out pixelHeight, out framePerSecond, out requestedPixelFormat);

            var rSource = _renderingSourceFactory.Create(pixelWidth, pixelHeight, requestedPixelFormat);
            eventAggregator.Publish(new RenderingSurfaceCreatedEvent(this.InstanceId, rSource.MemorySection, rSource.RenderingSurface, rSource.SurfaceInformation));

            _emulator.SetRenderingSurface(rSource.RenderingSurface);
            var input = _inputPipeline.BuildInputSource(context);

            eventAggregator.Publish(new EmulatorStartedEvent(this.InstanceId));

            FrameRateTimer fp = _frameRateTimerFactory.GetFrameRateTimer(framePerSecond);
            fp.ScheduleAction(() =>
            {
                while (_bus.HasMessages)
                {
                    _bus.GetCommand().Execute(_emulator);
                }

                _emulator.SetInput(input.GetCurrentInputState());

                Boolean isRunning;
                try
                {
                    isRunning = _emulator.Run();
                }
                catch
                {
                    isRunning = false;
                }

                eventAggregator.Publish(new FrameRenderedEvent(this.InstanceId));

                if (!isRunning)
                {
                    eventAggregator.Publish(new EmulatorQuitEvent(this.InstanceId));
                    input.Close();
                }

                return isRunning;
            });

            if (!fp.IsRunning)
            {
                fp.Start();
            }
        }
    }
}
