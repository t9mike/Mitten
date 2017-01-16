using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Mitten.Server.Events
{
    /// <summary>
    /// A basic event bus for sending and receiving events all within the current process.
    /// </summary>
    public class InProcessEventBus : IEventBus
    {
        private ConcurrentBag<IEventSubscriber> eventSubscribers;

        /// <summary>
        /// Initializes a new instance of the PublishingEventBus class.
        /// </summary>
        public InProcessEventBus()
        {
            this.eventSubscribers = new ConcurrentBag<IEventSubscriber>();
        }

        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="eventData">The event data to publish.</param>
        public async void Publish(IEvent eventData)
        {
            foreach (IEventSubscriber subscriber in this.eventSubscribers)
            {
                try
                {
                    await Task.Run(
                        () =>
                        {
                            EventEnvelope eventEnvelope =
                                new EventEnvelope(
                                    eventData.Name, 
                                    type =>
                                    {
                                        if (!type.IsAssignableFrom(eventData.GetType()))
                                        {
                                            throw new InvalidOperationException("Cannot assign event of Type (" + eventData.GetType().FullName + ") to Type (" + type.FullName + ").");
                                        }

                                        return eventData;
                                    });

                            subscriber.ProcessEvent(eventEnvelope);
                        })
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // TODO: is it possible this would cause a stack overflow if a subscriber throws when handling an error event?
                    this.Publish(new SystemErrorEvent("An unhandled exception was thrown by event " + eventData.Name + ".", ex));
                }
            }
        }

        /// <summary>
        /// Subscribes to events published to the bus.
        /// </summary>
        /// <param name="eventSubscriber">An event subscriber.</param>
        public void Subscribe(IEventSubscriber eventSubscriber)
        {
            this.eventSubscribers.Add(eventSubscriber);
        }
    }
}