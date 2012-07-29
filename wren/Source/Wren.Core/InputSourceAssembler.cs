using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class InputSourceAssembler : Wren.Core.IInputSourceAssembler
    {
        IList<Func<EmulationContext, IInputSource>> _inputSourceFactories;
        IList<Func<EmulationContext, IInputSource, IInputSource>> _inputSourceConfigurations;

        public InputSourceAssembler()
        {
            _inputSourceFactories = new List<Func<EmulationContext, IInputSource>>();
            _inputSourceConfigurations = new List<Func<EmulationContext, IInputSource, IInputSource>>();
        }

        public IInputSource BuildInputSource(EmulationContext context)
        {
            IInputSource inputSource = new CompositeInputSource(); ;

            foreach (var source in _inputSourceFactories)
            {
                var newSource = source.Invoke(context);
                if (newSource != null)
                {
                    ((CompositeInputSource)inputSource).AddInputSource(source.Invoke(context));
                }
            }

            foreach (var configuration in _inputSourceConfigurations)
            {
                inputSource = configuration.Invoke(context, inputSource);
            }

            return inputSource;
        }

        public void ConfigureInputSource(Func<EmulationContext, IInputSource> inputSourceFactory)
        {
            _inputSourceFactories.Add(inputSourceFactory);
        }

        public void ConfigurePipeline(Func<EmulationContext, IInputSource, IInputSource> configuration)
        {
            _inputSourceConfigurations.Add(configuration);
        }
    }
}
