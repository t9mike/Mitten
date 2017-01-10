using System;
using System.Threading;

namespace Mitten.Server.Commands.Tests.Unit
{
    /// <summary>
    /// A basic command used for testing.
    /// </summary>
    public class TestCommand : Command<string>, ITestCommand
    {
        /// <summary>
        /// Initialize a new instance of the TestCommand class.
        /// </summary>
        public TestCommand()
            : this(CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initialize a new instance of the TestCommand class.
        /// </summary>
        /// <param name="properties">The properties for the command.</param>
        public TestCommand(CommandProperties properties)
            : base("Test Group", properties)
        {
        }

        /// <summary>
        /// Initialize a new instance of the TestCommand class.
        /// </summary>
        /// <param name="commandName">A name for the command.</param>
        /// <param name="properties">The properties for the command.</param>
        public TestCommand(string commandName, CommandProperties properties)
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
        /// Gets the id of the thread the command was executed on.
        /// </summary>
        public int ExecutionThreadId { get; private set; }

        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <returns>The command response.</returns>
        protected override string Run()
        {
            this.ExecutionThreadId = Thread.CurrentThread.ManagedThreadId;

            if (this.ExecutionDelay > 0)
            {
                Thread.Sleep(this.ExecutionDelay);
            }

            if (this.ExceptionToThrow != null)
            {
                throw this.ExceptionToThrow;
            }

            return this.Response;
        }
    }
}
