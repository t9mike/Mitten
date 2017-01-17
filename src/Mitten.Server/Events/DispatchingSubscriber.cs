using System;
using System.Collections.Concurrent;

namespace Mitten.Server.Events
{
    /// <summary>
    /// An event subscriber that dispatches events to registered handlers.
    /// </summary>
    public class DispatchingSubscriber : IEventSubscriber
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<Action<EventEnvelope>>> handlers;

        /// <summary>
        /// Initializes a new instance of the DispatchingSubscriber class.
        /// </summary>
        public DispatchingSubscriber()
        {
            this.handlers = new ConcurrentDictionary<string, ConcurrentBag<Action<EventEnvelope>>>();
        }

        /// <summary>
        /// Processes an received event. 
        /// </summary>
        /// <param name="eventEnvelope">An event envelope.</param>
        public void ProcessEvent(EventEnvelope eventEnvelope)
        {
            ConcurrentBag<Action<EventEnvelope>> eventHandlers;
            if (this.handlers.TryGetValue(eventEnvelope.EventName, out eventHandlers))
            {
                foreach (Action<EventEnvelope> handler in eventHandlers)
                {
                    handler(eventEnvelope);
                }
            }
        }

        /// <summary>
        /// Registers a handler to handle the specified event.
        /// </summary>
        /// <param name="handler">An event handler.</param>
        public void Register<TEventData>(Action<TEventData> handler)
            where TEventData : IEvent
        {
            this.Register<TEventData>(null, handler);
        }

        /// <summary>
        /// Registers a handler to handle the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event, if null the name of the class will be used as the event name.</param>
        /// <param name="handler">An event handler.</param>
        public void Register<TEvent>(string eventName, Action<TEvent> handler)
            where TEvent : IEvent
        {
            Throw.IfArgumentNull(handler, nameof(handler));

            if (string.IsNullOrWhiteSpace(eventName))
            {
                eventName = typeof(TEvent).Name;
            }

            ConcurrentBag<Action<EventEnvelope>> eventHandlers = 
                this.handlers.GetOrAdd(
                    eventName,
                    type => new ConcurrentBag<Action<EventEnvelope>>());

            eventHandlers.Add(eventEnvelope => handler(eventEnvelope.GetContent<TEvent>()));
        }

        /// <summary>
        /// Registers a handler to handle the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event, if null the name of the class will be used as the event name.</param>
        /// <param name="handler">An event handler.</param>
        public void Register(string eventName, Action<dynamic> handler)
        {
            Throw.IfArgumentNullOrWhitespace(eventName, nameof(eventName));
            Throw.IfArgumentNull(handler, nameof(handler));

            ConcurrentBag<Action<EventEnvelope>> eventHandlers =
                this.handlers.GetOrAdd(
                    eventName,
                    type => new ConcurrentBag<Action<EventEnvelope>>());

            eventHandlers.Add(eventEnvelope => handler(eventEnvelope.GetContent()));
        }
    }
}