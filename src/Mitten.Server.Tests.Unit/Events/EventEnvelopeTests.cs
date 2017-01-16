using System;
using FluentAssertions;
using Mitten.Server.Events;
using Mitten.Server.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit.Events
{
    [TestFixture]
    public class EventEnvelopeTests
    {
        [Test]
        public void GetStronglyTypedContentTest()
        {
            TestEvent testEvent = new TestEvent("some event");
            EventEnvelope envelope = new EventEnvelope("Test", type => testEvent);

            TestEvent content = envelope.GetContent<TestEvent>();

            content.Should().NotBeNull();
            content.ShouldBeEquivalentTo(testEvent);
        }

        [Test]
        public void GetStronglyTypedContentFromJsonTest()
        {
            TestEvent testEvent = new TestEvent("some event");
            string json = JsonConvert.SerializeObject(testEvent);
            EventEnvelope envelope = new EventEnvelope("Test", type => this.FromJson(type, json));

            TestEvent content = envelope.GetContent<TestEvent>();

            content.Should().NotBeNull();
            content.EventDate.ShouldBeEquivalentTo(testEvent.EventDate);
            content.EventId.ShouldBeEquivalentTo(testEvent.EventId);
            content.Name.ShouldBeEquivalentTo(testEvent.Name);
            content.Value.ShouldBeEquivalentTo(testEvent.Value);
        }

        [Test]
        public void GetDynamicContentFromJsonTest()
        {
            TestEvent testEvent = new TestEvent("some event");
            string json = JsonConvert.SerializeObject(testEvent);
            EventEnvelope envelope = new EventEnvelope("Test", type => this.FromJson(type, json));

            dynamic content = envelope.GetContent();

            ((DateTime)content.EventDate).ShouldBeEquivalentTo(testEvent.EventDate);
            Assert.AreEqual(testEvent.EventId, Guid.Parse(content.EventId));
            ((string)content.Name).ShouldBeEquivalentTo(testEvent.Name);
            ((string)content.Value).ShouldBeEquivalentTo(testEvent.Value);
        }

        private object FromJson(Type expectedType, string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new PrivateSetterContractResolver();

            return JsonConvert.DeserializeObject(json, expectedType, settings);
        }
    }
}
