using System;

namespace Mitten.Server.Commands.Tests.Unit
{
    /// <summary>
    /// A command used to test the execution context.
    /// </summary>
    public interface ITestCommand
    {
        /// <summary>
        /// Gets or sets an exception that should be thrown during the execution of the command.
        /// </summary>
        Exception ExceptionToThrow { get; set; }

        /// <summary>
        /// Gets or sets the response that should be returned from the command.
        /// </summary>
        string Response { get; set; }

        /// <summary>
        /// Gets or sets the execution delay for the command.
        /// </summary>
        int ExecutionDelay { get; set; }

        /// <summary>
        /// Gets the id of the thread the command was executed on.
        /// </summary>
        int ExecutionThreadId { get; }
    }
}
