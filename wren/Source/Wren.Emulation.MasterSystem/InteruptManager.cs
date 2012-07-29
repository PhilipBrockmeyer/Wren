using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class InteruptManager : IInteruptManager
    {
        IDictionary<Interupts, IList<InteruptDelegate>> _handlers;

        public InteruptManager()
        {
            _handlers = new Dictionary<Interupts, IList<InteruptDelegate>>();

            foreach (var interuptType in Enum.GetValues(typeof(Interupts)))
            {
                _handlers[(Interupts)interuptType] = new List<InteruptDelegate>();
            }
        }

        public void RegisterInteruptHandler(Interupts interupt, InteruptDelegate handler)
        {
            _handlers[interupt].Add(handler);
        }

        public void RunInteruptHandlers(Interupts interupt)
        {
            foreach (var handler in _handlers[interupt])
            {
                handler();
            }
        }

        public void Clear()
        {
            foreach (var handlerList in _handlers.Values)
                handlerList.Clear();
        }
    }
}
