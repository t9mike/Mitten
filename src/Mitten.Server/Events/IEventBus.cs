using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Provides methods sending and receiving events using a pub/sub model.
    /// </summary>
    public interface IEventBus : IEventPublisher
    {
        /// <summary>
        /// Subscribes to events published to the bus.
        /// </summary>
        /// <param name="eventSubscriber">An event subscriber.</param>
        void Subscribe(IEventSubscriber eventSubscriber);
    }
}