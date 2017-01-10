using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mitten.Server.Commands.Tests.Unit
{
    /// <summary>
    /// A basic async command used for testing.
    /// </summary>
    public class TestAsyncCommand : AsyncCommand<string>, ITestCommand
    {
        /// <summary>
        /// Initialize a new instance of the TestAsyncCommand class.
        /// </summary>
        public TestAsyncCommand()
            : this(CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initialize a new instance of the TestAsyncCommand class.
        /// </summary>
        /// <param name="properties">The properties for the command.</param>
        public TestAsyncCommand(CommandProperties properties)
            : base("Test Group", properties)
        {
        }

        /// <summary>
        /// Initialize a new instance of the TestAsyncCommand class.
        /// </summary>
        /// <param name="commandName">A name for the command.</param>
        /// <param name="properties">The properties for the command.</param>
        public TestAsyncCommand(string commandName, CommandProperties properties)
            : base("Test Group", commandName, properties)
        {
        }

        /// <summary>
        /// Gets or sets an exception that should be thrown during the execution of the command.
        /// </summary>
        public Exception ExceptionToThrow { get; set; }

        /// <summary>
        /// Gets or sets the response that should be returned from the command.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the execution delay for the command.
        /// </summary>
        public int ExecutionDelay { get; set; }

        /// <summary>
        /// Gets or sets whether or not the execution delay should be applied prior to executing the async Task
        /// or if the command should return immediately and instead apply the delay to the async Task.
        /// </summary>
        public bool DelayExecutionTask { get; set; }

        /// <summary>
        /// Gets the id of the thread the command was executed on.
        /// </summary>
        public int ExecutionThreadId { get; private set; }

        /// <summary>
        /// Runs the command logic asynchronously and returns a Task.
        /// </summary>
        /// <returns>The Task from the command.</returns>
        protected override async Task<string> RunAsync()
        {
            this.ExecutionThreadId = Thread.CurrentThread.ManagedThreadId;

            if (this.ExecutionDelay > 0 && !this.DelayExecutionTask)
            {
                Thread.Sleep(this.ExecutionDelay);
            }

            if (this.ExceptionToThrow != null)
            {
                throw this.ExceptionToThrow;
            }

            if (this.ExecutionDelay > 0 && this.DelayExecutionTask)
            {
                await Task.Delay(this.ExecutionDelay).ConfigureAwait(false);
            }

            return this.Response;
        }
    }
}
