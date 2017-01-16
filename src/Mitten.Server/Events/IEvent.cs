using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the id for the event.
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the date and time the event was created.
        /// </summary>
        DateTime EventDate { get; }
    }
}
