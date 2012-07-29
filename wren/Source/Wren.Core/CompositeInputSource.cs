using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class CompositeInputSource : IInputSource
    {
        IList<IInputSource> _inputSources;

        public CompositeInputSource()
        {
            _inputSources = new List<IInputSource>();
        }

        public void AddInputSource(IInputSource inputSource)
        {
            _inputSources.Add(inputSource);
        }

        public InputState GetCurrentInputState()
        {
            InputState state = new InputState();

            foreach (var source in _inputSources)
            {
                state.Merge(source.GetCurrentInputState());
            }
            
            return state;
        }

        public void Open()
        { }

        public void Close()
        { }
    }
}
