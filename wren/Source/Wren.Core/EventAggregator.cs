using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Wren.Core;
using System.Threading;

namespace Wren.Core
{
    public class EventAggregator : IEventAggregator
    {
        Object _lockObject = new Object();

        ServiceLocator _serviceLocator;
        IDictionary<Type, List<Type>> _eventSubscriptions;
        IDictionary<Type, List<Action<IEvent>>> _eventActionSubscriptions;

        public EventAggregator(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _eventSubscriptions = new Dictionary<Type, List<Type>>();
            _eventActionSubscriptions = new Dictionary<Type, List<Action<IEvent>>>();
        }

        public void Publish(IEvent e)
        {
            List<Type> handlerTypes = null;
            List<Action<IEvent>> handlers = null;

            lock (_lockObject)
            {
                if (_eventSubscriptions.ContainsKey(e.GetType()))
                    handlerTypes = new List<Type>(_eventSubscriptions[e.GetType()]);

                if (_eventActionSubscriptions.ContainsKey(e.GetType()))
                    handlers = new List<Action<IEvent>>(_eventActionSubscriptions[e.GetType()]);
            }


            if (handlerTypes != null)
            {

                foreach (var handlerType in handlerTypes)
                {
                    var handler = (IEventHandler)_serviceLocator.GetInstance(handlerType);
                    handler.Execute(e);
                }
            }

            if (handlers != null)
            {
                try
                {
                    foreach (var handlerAction in handlers)
                    {
                        handlerAction.Invoke(e);
                    }
                }
                catch
                {
                }
            }
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            lock (_lockObject)
            {
                if (!_eventSubscriptions.ContainsKey(typeof(TEvent)))
                {
                    _eventSubscriptions.Add(typeof(TEvent), new List<Type>());
                }

                _eventSubscriptions[typeof(TEvent)].Add(typeof(TEventHandler));
            }
        }

        public void Subscribe<TEvent>(Action<IEvent> action)
            where TEvent : IEvent
        {
            lock (_lockObject)
            {
                if (!_eventActionSubscriptions.ContainsKey(typeof(TEvent)))
                {
                    _eventActionSubscriptions.Add(typeof(TEvent), new List<Action<IEvent>>());
                }

                _eventActionSubscriptions[typeof(TEvent)].Add((Action<IEvent>)action);
            }
        }
    }
}
