using System;
namespace Wren.Core
{
    public interface IEventAggregator
    {
        void Publish(IEvent e);
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;

        void Subscribe<TEvent>(Action<IEvent> action)
            where TEvent : IEvent;

    }
}
