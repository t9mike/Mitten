using System.Collections.Generic;
using FluentAssertions;
using Mitten.Server.Events;
using NUnit.Framework;

namespace Mitten.Server.Commands.Tests.Unit
{
    [TestFixture]
    public class CommandExecutionMetricsTests
    {
        [TestCase]
        public void CurrentExecutionCountTest()
        {
            IEventBus eventBus = new TestEventBus();
            EventDispatcher dispatcher = new EventDispatcher(eventBus);
            CommandExecutionMetrics metrics = new CommandExecutionMetrics();

            metrics.RegisterWithDispatcher(dispatcher);

            TestCommand command = new TestCommand();
            eventBus.Publish(new CommandExecutionStartedEvent(command.CommandKey));

            metrics.CurrentExecutionCount.ShouldBeEquivalentTo(1);

            CommandResult<string> result = this.CreateSuccessResult(command);
            eventBus.Publish(new CommandExecutedEvent(command.CommandKey, result));

            metrics.CurrentExecutionCount.ShouldBeEquivalentTo(0);
        }

        [TestCase]
        public void SuccessfulEventCountTest()
        {
            IEventBus eventBus = new TestEventBus();
            EventDispatcher dispatcher = new EventDispatcher(eventBus);
            CommandExecutionMetrics metrics = new CommandExecutionMetrics();

            metrics.RegisterWithDispatcher(dispatcher);

            TestCommand command = new TestCommand();
            CommandResult<string> result = this.CreateSuccessResult(command);

            eventBus.Publish(new CommandExecutionStartedEvent(command.CommandKey));
            eventBus.Publish(new CommandExecutedEvent(command.CommandKey, result));

            CommandExecutionMetrics.EventCountsSnapshot snapshot = metrics.GetCommandEventCounts(command.CommandKey);

            snapshot.GetCount(CommandExecutionEventType.Success).ShouldBeEquivalentTo(1);
        }

        [TestCase]
        public void MultipleEventCountsTest()
        {
            IEventBus eventBus = new TestEventBus();
            EventDispatcher dispatcher = new EventDispatcher(eventBus);
            CommandExecutionMetrics metrics = new CommandExecutionMetrics();

            metrics.RegisterWithDispatcher(dispatcher);

            TestCommand command = new TestCommand();

            const int successCount = 5;
            const int badRequestCount = 3;
            const int failureCount = 0;

            for (int i = 0; i < successCount; i++)
            {
                this.PublishEvents(eventBus, command, CommandExecutionEventType.Success);
            }

            for (int i = 0; i < badRequestCount; i++)
            {
                this.PublishEvents(eventBus, command, CommandExecutionEventType.BadRequest);
            }

            for (int i = 0; i < failureCount; i++)
            {
                this.PublishEvents(eventBus, command, CommandExecutionEventType.CommandException);
            }

            CommandExecutionMetrics.EventCountsSnapshot snapshot = metrics.GetCommandEventCounts(command.CommandKey);

            snapshot.GetCount(CommandExecutionEventType.Success).ShouldBeEquivalentTo(successCount);
            snapshot.GetCount(CommandExecutionEventType.BadRequest).ShouldBeEquivalentTo(badRequestCount);
            snapshot.GetCount(CommandExecutionEventType.CommandException).ShouldBeEquivalentTo(failureCount);
        }

        private void PublishEvents(IEventBus eventBus, TestCommand command, CommandExecutionEventType eventType)
        {
            CommandResult<string> result = this.CreateResult(command, eventType);

            eventBus.Publish(new CommandExecutionStartedEvent(command.CommandKey));
            eventBus.Publish(new CommandExecutedEvent(command.CommandKey, result));
        }

        private CommandResult<string> CreateSuccessResult(TestCommand command)
        {
            return this.CreateResult(command, CommandExecutionEventType.Success);
        }

        private CommandResult<string> CreateResult(TestCommand command, CommandExecutionEventType eventType)
        {
            CommandResult<string> result = new CommandResult<string>(command.GroupName, command.CommandName);

            result.SetResponse(string.Empty);
            result.SignalExecutionStarted();
            result.SignalExecutionDone(eventType);

            return result;
        }

        private class TestEventBus : IEventBus
        {
            private readonly List<IEventSubscriber> subscribers = new List<IEventSubscriber>();
            
            public void Publish(EventData eventData)
            {
                foreach (IEventSubscriber subscriber in subscribers)
                {
                    subscriber.ProcessEvent(eventData);
                }
            }

            public void Subscribe(IEventSubscriber eventSubscriber)
            {
                this.subscribers.Add(eventSubscriber);
            }
        }
    }
}
