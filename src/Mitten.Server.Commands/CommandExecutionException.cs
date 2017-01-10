using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents an error that occurred during the execution of a command.
    /// </summary>
    public class CommandExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CommandExecutionException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">An exception that caused the current error.</param>
        internal CommandExecutionException(CommandFailureType failureType, string message, Exception innerException)
            : base (message, innerException)
        {
            this.FailureType = failureType;
        }

        /// <summary>
        /// Gets the failure type.
        /// </summary>
        public CommandFailureType FailureType { get; private set; }
    }
}