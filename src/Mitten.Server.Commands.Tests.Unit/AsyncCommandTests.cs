using System;
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mitten.Server.Commands.Tests.Unit
{
    [TestFixture]
    public class AsyncCommandTests
    {
        [Test]
        public void ObservableOnNextTest()
        {
            TestAsyncCommand command = new TestAsyncCommand();
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
            TestAsyncCommand command = new TestAsyncCommand();

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
            TestAsyncCommand command = new TestAsyncCommand();

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
            TestAsyncCommand command = new TestAsyncCommand();

            bool wasCompleted = false;
            command.GetExecutionObservable().Subscribe(_ => { }, () => wasCompleted = true);

            wasCompleted.Should().BeTrue();
        }
    }
}
