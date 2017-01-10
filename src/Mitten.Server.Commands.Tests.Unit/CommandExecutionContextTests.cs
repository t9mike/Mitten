using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Mitten.Server.Commands.Tests.Unit
{
    [TestFixture]
    public class CommandExecutionContextTests
    {
        [Test]
        public void ExecuteSuccessfulCommandTest()
        {
            this.RunExecuteCommandWithSuccessfulResultTest<TestCommand>();
        }

        [Test]
        public void ExecuteSuccessfulFunctionCommandTest()
        {
            CommandExecutionContext context = new CommandExecutionContext();
            const string response = "test";

            CommandResult<string> result = context.Execute("TestCommand", () => response).Result;

            result.IsSuccessful.Should().BeTrue();
            result.Response.ShouldBeEquivalentTo(response);
            result.Exception.Should().BeNull();

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.Success);
        }

        [Test]
        public void ExceptionThrownInFunctionCommandTest()
        {
            CommandExecutionContext context = new CommandExecutionContext();

            CommandResult<string> result = 
                context.Execute(
                    "TestCommand", 
                    () => 
                    {
                        object obj = null;
                        return obj.ToString();
                    })
               .Result;

            result.IsSuccessful.Should().BeFalse();
            result.Exception.Should().BeOfType<CommandExecutionException>();

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.CommandException);
        }

        [Test]
        public void ExecuteSuccessfulActionCommandTest()
        {
            CommandExecutionContext context = new CommandExecutionContext();

            CommandResult result = context.Execute("TestCommand", () => { }).Result;

            result.IsSuccessful.Should().BeTrue();
            result.Exception.Should().BeNull();

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.Success);
        }

        [Test]
        public void CommandTimeoutTest()
        {
            this.RunCommandTimeoutTest(properties => new TestCommand(properties));
        }

        [Test]
        public void InvalidOperationFromCommandExceptionTest()
        {
            this.RunInvalidOperationFromCommandExceptionTest<TestCommand>();
        }

        [Test]
        public void RunBadRequestFromCommandTest()
        {
            this.RunBadRequestFromCommandTest<TestCommand>();
        }

        [Test]
        public void ExecuteAsyncCommandWithSuccessfulResultTest()
        {
            this.RunExecuteCommandWithSuccessfulResultTest<TestAsyncCommand>();
        }

        [Test]
        public void AsyncCommandTimeoutTest()
        {
            this.RunCommandTimeoutTest(properties => new TestAsyncCommand(properties));
        }

        [Test]
        public void AsyncCommandTimeoutInTaskTest()
        {
            this.RunCommandTimeoutTest(properties =>
            {
                TestAsyncCommand command = new TestAsyncCommand(properties);
                command.DelayExecutionTask = true;
                return command;
            });
        }

        [Test]
        public void AsyncInvalidOperationFromCommandExceptionTest()
        {
            this.RunInvalidOperationFromCommandExceptionTest<TestAsyncCommand>();
        }

        [Test]
        public void AsyncRunBadRequestFromCommandTest()
        {
            this.RunBadRequestFromCommandTest<TestAsyncCommand>();
        }

        private void RunExecuteCommandWithSuccessfulResultTest<TCommand>()
            where TCommand : BaseCommand<string>, ITestCommand
        {
            TCommand command = Activator.CreateInstance<TCommand>();
            const string value = "test value";

            command.Response = value;

            CommandResult<string> result = this.Execute(command).Result;

            result.IsSuccessful.Should().BeTrue();
            result.Response.ShouldBeEquivalentTo(value);
            result.Exception.Should().BeNull();

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.Success);
        }

        private void RunCommandTimeoutTest<TCommand>(Func<CommandProperties, TCommand> createCommand)
            where TCommand : BaseCommand<string>, ITestCommand
        {
            CommandProperties properties = new CommandProperties(10);
            TCommand command = createCommand(properties);

            command.ExecutionDelay = 100;

            CommandResult<string> result = this.Execute(command).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Should().BeOfType<CommandExecutionException>();

            ((CommandExecutionException)result.Exception).FailureType.ShouldBeEquivalentTo(CommandFailureType.Timeout);

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.Timeout);
        }

        private void RunInvalidOperationFromCommandExceptionTest<TCommand>()
            where TCommand : BaseCommand<string>, ITestCommand
        {
            TCommand command = Activator.CreateInstance<TCommand>();
            command.ExceptionToThrow = new InvalidOperationException("Invalid Operation");

            CommandResult<string> result = this.Execute(command).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Should().BeOfType<CommandExecutionException>();

            ((CommandExecutionException)result.Exception).FailureType.ShouldBeEquivalentTo(CommandFailureType.CommandException);

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.CommandException);
        }

        private void RunBadRequestFromCommandTest<TCommand>()
            where TCommand : BaseCommand<string>, ITestCommand
        {
            TCommand command = Activator.CreateInstance<TCommand>();
            command.ExceptionToThrow = new BadRequestException("test", "Bad Request");

            CommandResult<string> result = this.Execute(command).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Should().BeOfType<BadRequestException>();

            result.Events.Should().HaveCount(1);
            result.Events.Single().ShouldBeEquivalentTo(CommandExecutionEventType.BadRequest);
        }

        private Task<CommandResult<string>> Execute(BaseCommand<string> command)
        {
            CommandExecutionContext context = new CommandExecutionContext();
            return context.Execute(command);
        }
    }
}
