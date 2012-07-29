using System;
namespace Wren.Core
{
    public interface IInputSourceAssembler
    {
        IInputSource BuildInputSource(EmulationContext context);
        void ConfigureInputSource(Func<EmulationContext, IInputSource> inputSourceFactory);
        void ConfigurePipeline(Func<EmulationContext, IInputSource, IInputSource> configuration);
    }
}
