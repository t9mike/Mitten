using System;
using Mitten.Server.Events;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// A publisher that ignores published events.
    /// </summary>
    internal class NoOpEventPublisher : IEventPublisher
    {
        /// <summary>
        /// An instance of the publisher.
        /// </summary>
        public static readonly NoOpEventPublisher Instance = new NoOpEventPublisher();

        private NoOpEventPublisher()
        {
        }

        void IEventPublisher.Publish(IEvent eventData)
        {
        }
    }
}
