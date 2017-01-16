using System;
using System.Threading;
using FluentAssertions;
using Mitten.Server.Events;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit.Events
{
    [TestFixture]
    public class InProcessEventBusTests
    {
        [Test]
        public void PublishAndSubscribeEventTest()
        {
            InProcessEventBus bus = new InProcessEventBus();
            TestEvent testEvent = new TestEvent("some value");

            TestEvent receivedEvent = null;
            bus.Subscribe(new Subscriber(eventEnvelope => receivedEvent = eventEnvelope.GetContent<TestEvent>()));

            bus.Publish(testEvent);

            // sleep because the bus publishes events on another thread
            Thread.Sleep(10);

            receivedEvent.Should().NotBeNull();
            receivedEvent.ShouldBeEquivalentTo(testEvent);
        }

        private class Subscriber : IEventSubscriber
        {
            private readonly Action<EventEnvelope> handleEvent;

            public Subscriber(Action<EventEnvelope> handleEvent)
            {
                this.handleEvent = handleEvent;
            }

            public void ProcessEvent(EventEnvelope eventEnvelope)
            {
                this.handleEvent(eventEnvelope);
            }
        }
    }
}
