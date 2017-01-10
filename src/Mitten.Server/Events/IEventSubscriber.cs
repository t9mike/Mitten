namespace Mitten.Server.Events
{
    /// <summary>
    /// Handles subscribing to and processing published events.
    /// </summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Processes an received event. 
        /// </summary>
        /// <param name="eventData">The event data.</param>
        void ProcessEvent(EventData eventData);
    }
}