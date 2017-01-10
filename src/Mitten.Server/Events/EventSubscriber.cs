using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// A basic event subscriber that routes events to a callback.
    /// </summary>
    public class EventSubscriber : IEventSubscriber
    {
        private readonly Action<EventData> processEvent;

        /// <summary>
        /// Initializes a new instance of the EventSubscriber class.
        /// </summary>
        /// <param name="processEvent">Process event.</param>
        public EventSubscriber(Action<EventData> processEvent)
        {
            this.processEvent = processEvent;
        }

        /// <summary>
        /// Processes an received event. 
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void ProcessEvent(EventData eventData)
        {
            this.processEvent(eventData);
        }
    }
}