using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Contains the data sent for an event.
    /// </summary>
    public abstract class EventData
    {
        private static class Constants
        {
            public const string EventSuffix = "Event";
        }

        /// <summary>
        /// Initializes a new instance of the EventData class.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        protected EventData(EventType eventType)
            : this(null, eventType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EventData class.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="eventType">The type of event.</param>
        protected EventData(string name, EventType eventType)
        {
            this.Name = 
                string.IsNullOrWhiteSpace(name)
                ? this.GetEventName()
                : name;

            this.EventType = eventType;
            this.EventDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the type of event.
        /// </summary>
        public EventType EventType { get; }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the date and time the event was created.
        /// </summary>
        public DateTime EventDate { get; }

        private string GetEventName()
        {
            string name = this.GetType().Name;

            if (!name.EndsWith(Constants.EventSuffix, StringComparison.Ordinal))
            {
                return name;
            }

            return name.Remove(name.LastIndexOf(Constants.EventSuffix, StringComparison.Ordinal));
        }
    }
}