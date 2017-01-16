using Mitten.Server.Events;

namespace Mitten.Server.Tests.Unit.Events
{
    public class TestEvent : EventBase
    {
        public TestEvent(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}
