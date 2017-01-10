using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents an unhandled exception thrown from the execution of a command.
    /// </summary>
    internal class UnhandledCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnhandledCommandException class.
        /// </summary>
        /// <param name="innerException">Inner exception.</param>
        public UnhandledCommandException(Exception innerException)
            : base ("Unhandled exception in command.", innerException)
        {
        }
    }
}