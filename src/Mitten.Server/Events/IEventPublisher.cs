namespace Mitten.Server.Events
{
    /// <summary>
    /// Handles publishing events.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="eventData">The event data to publish.</param>
        void Publish(IEvent eventData);
    }
}