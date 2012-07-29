using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public interface IEventHandler
    {
        void Execute(Object e);
    }

    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : IEvent
    {
        void Execute(TEvent e);
    }

    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        public abstract void Execute(TEvent e);

        public void Execute(object e)
        {
            Execute((TEvent)e);
        }
    }
}
