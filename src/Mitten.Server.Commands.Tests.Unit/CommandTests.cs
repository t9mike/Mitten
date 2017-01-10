using System;
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mitten.Server.Commands.Tests.Unit
{
    [TestFixture]
    public class CommandTests
    {
        [Test]
        public void CommandNameTest()
        {
            TestCommand command = new TestCommand();
            command.CommandName.ShouldBeEquivalentTo("TestCommand");
        }

        [Test]
        public void AcquireForExecutionTest()
        {
            TestCommand command = new TestCommand();

            command.AcquireForExecution(new CommandExecutionContext()).Should().BeTrue();
            command.AcquireForExecution(new CommandExecutionContext()).Should().BeFalse();
        }

        [Test]
        public void ObservableOnNextTest()
        {
            TestCommand command = new TestCommand();
            const string expectedResponse = "test";

            command.Response = expectedResponse;
            IObservable<string> observable = command.GetExecutionObservable();

            string actualResponse = null;
            observable.Subscribe(response => actualResponse = response);

            actualResponse.ShouldBeEquivalentTo(expectedResponse);
        }

        [Test]
        public void ObservableOnUnhandledCommandExceptionTest()
        {
            TestCommand command = new TestCommand();

            command.ExceptionToThrow = new InvalidOperationException("test error");
            IObservable<string> observable = command.GetExecutionObservable();

            Exception actualException = null;
            observable.Subscribe(_ => { }, ex => actualException = ex);

            actualException.Should().NotBeNull();
            actualException.Should().BeOfType<UnhandledCommandException>();
            actualException.InnerException.Should().BeOfType<InvalidOperationException>();
        }

        [Test]
        public void ObservableOnBadRequestExceptionTest()
        {
            TestCommand command = new TestCommand();

            command.ExceptionToThrow = new BadRequestException("test", "test error");
            IObservable<string> observable = command.GetExecutionObservable();

            Exception actualException = null;
            observable.Subscribe(_ => { }, ex => actualException = ex);

            actualException.Should().NotBeNull();
            actualException.Should().BeOfType<BadRequestException>();
        }

        [Test]
        public void ObservableOnCompletedTest()
        {
            TestCommand command = new TestCommand();

            bool wasCompleted = false;
            command.GetExecutionObservable().Subscribe(_ => { }, () => wasCompleted = true);

            wasCompleted.Should().BeTrue();
        }
    }
}
