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
        /// <param name="eventEnvelope">An event envelope.</param>
        void ProcessEvent(EventEnvelope eventEnvelope);
    }
}