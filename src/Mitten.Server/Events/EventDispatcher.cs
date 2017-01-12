using System;
using System.Collections.Concurrent;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Subscribes to events from a message bus and dispatches them to registered event handlers.
    /// </summary>
    public class EventDispatcher
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<Action<EventData>>> handlers;

        /// <summary>
        /// Initializes a new instance of the EventDispatcher class.
        /// </summary>
        /// <param name="eventBus">The event bus to subscribe to.</param>
        public EventDispatcher(IEventBus eventBus)
        {
            Throw.IfArgumentNull(eventBus, nameof(eventBus));

            this.handlers = new ConcurrentDictionary<Type, ConcurrentBag<Action<EventData>>>();
            eventBus.Subscribe(new EventSubscriber(this.ProcessEvent));
        }

        /// <summary>
        /// Registers a handler to handle the specified event.
        /// </summary>
        /// <param name="handler">An event handler.</param>
        public void Register<TEventData>(Action<TEventData> handler)
            where TEventData : EventData
        {
            Throw.IfArgumentNull(handler, "handler");

            ConcurrentBag<Action<EventData>> eventHandlers = 
                this.handlers.GetOrAdd(
                    typeof(TEventData),
                    type => new ConcurrentBag<Action<EventData>>());

            eventHandlers.Add(eventData => handler((TEventData)eventData));
        }

        private void ProcessEvent(EventData eventData)
        {
            ConcurrentBag<Action<EventData>> eventHandlers;
            if (this.handlers.TryGetValue(eventData.GetType(), out eventHandlers))
            {
                foreach (Action<EventData> handler in eventHandlers)
                {
                    handler(eventData);
                }
            }
        }
    }
}