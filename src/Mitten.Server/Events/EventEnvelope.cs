using System;
using System.Dynamic;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Represents the contents for an event. 
    /// </summary>
    public class EventEnvelope
    {
        private readonly Func<Type, object> getEventData;

        /// <summary>
        /// Initializes a new instance of the EventEnvelope class.
        /// </summary>
        /// <param name="eventName">The name of the event in the envelope.</param>
        /// <param name="getEventData">A callback that will return the data based on an expected Type.</param>
        public EventEnvelope(string eventName, Func<Type, object> getEventData)
        {
            Throw.IfArgumentNullOrWhitespace(eventName, nameof(eventName));
            Throw.IfArgumentNull(getEventData, nameof(getEventData));

            this.EventName = eventName;
            this.getEventData = getEventData;
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// Gets the content of the envelope.
        /// </summary>
        /// <returns>The event.</returns>
        public dynamic GetContent()
        {
            return this.getEventData(typeof(ExpandoObject));
        }

        /// <summary>
        /// Gets the content of the envelope.
        /// </summary>
        /// <typeparam name="TEvent">The type of event that was expected.</typeparam>
        /// <returns>The event.</returns>
        public TEvent GetContent<TEvent>()
            where TEvent : IEvent
        {
            return (TEvent)this.getEventData(typeof(TEvent));
        } 
    }
}
