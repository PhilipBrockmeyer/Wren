using System;
namespace Wren.Core
{
    public interface IEmulationRunner
    {
        Guid InstanceId { get; }
        void Start(EmulationContext context, IEventAggregator eventAggregator, EmulationMode mode);
        void SendCommand(IEmulatorCommand command);
        IEmulator Emulator { get; }
    }
}
