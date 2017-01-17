using System;
using FluentAssertions;
using Mitten.Server.Events;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit.Events
{
    [TestFixture]
    public class DispatchingSubscriberTests
    {
        [Test]
        public void RegisterEventHandlerByTypeTest()
        {
            DispatchingSubscriber subscriber = new DispatchingSubscriber();
            TestEvent testEvent = new TestEvent("some value");

            TestEvent receivedEvent = null;
            subscriber.Register<TestEvent>(evt => receivedEvent = evt);

            subscriber.ProcessEvent(new EventEnvelope(testEvent.Name, type => testEvent));

            receivedEvent.Should().NotBeNull();
            receivedEvent.ShouldBeEquivalentTo(testEvent);
        }

        [Test]
        public void RegisterEventHandlerByNameTest()
        {
            DispatchingSubscriber subscriber = new DispatchingSubscriber();
            TestEvent testEvent = new TestEvent("some value");

            TestEvent receivedEvent = null;
            subscriber.Register<TestEvent>("TestEvent", evt => receivedEvent = evt);

            subscriber.ProcessEvent(new EventEnvelope(testEvent.Name, type => testEvent));

            receivedEvent.Should().NotBeNull();
            receivedEvent.ShouldBeEquivalentTo(testEvent);
        }

        [Test]
        public void RegisterDynamicEventHandlerTest()
        {
            DispatchingSubscriber subscriber = new DispatchingSubscriber();
            TestEvent testEvent = new TestEvent("some value");

            dynamic receivedEvent = null;
            subscriber.Register("TestEvent", evt => receivedEvent = evt);

            subscriber.ProcessEvent(new EventEnvelope(testEvent.Name, type => testEvent));

            ((DateTime)receivedEvent.EventDate).ShouldBeEquivalentTo(testEvent.EventDate);
            ((Guid)receivedEvent.EventId).ShouldBeEquivalentTo(testEvent.EventId);
            ((string)receivedEvent.Name).ShouldBeEquivalentTo(testEvent.Name);
            ((string)receivedEvent.Value).ShouldBeEquivalentTo(testEvent.Value);
        }
    }
}
