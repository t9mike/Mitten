using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Base class for a strongly-typed event.
    /// </summary>
    public abstract class EventBase : IEvent
    {
        /// <summary>
        /// Initializes a new instance of the EventBase class.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        protected EventBase(string name = null)
        {
            this.Name = 
                string.IsNullOrWhiteSpace(name)
                ? this.GetType().Name
                : name;

            this.EventId = Guid.NewGuid();
            this.EventDate = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Gets the id for the event.
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the date and time the event was created.
        /// </summary>
        public DateTime EventDate { get; private set; }
    }
}