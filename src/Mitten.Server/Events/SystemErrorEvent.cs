using System;

namespace Mitten.Server.Events
{
    /// <summary>
    /// Represents an error that occurred somewhere in the system.
    /// </summary>
    public sealed class SystemErrorEvent : SystemEvent
    {
        /// <summary>
        /// Initializes a new instance of the SystemErrorEvent class.
        /// </summary>
        /// <param name="description">A description of the error.</param>
        public SystemErrorEvent(string description)
            : this(description, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SystemErrorEvent class.
        /// </summary>
        /// <param name="description">A description of the error.</param>
        /// <param name="exception">An exception if the error was due to a thrown exception, otherwise null.</param>
        public SystemErrorEvent(string description, Exception exception)
        {
            Throw.IfArgumentNullOrWhitespace(description, nameof(description));

            this.Description = description;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets a description of the error.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets an exception if the error was due to a thrown exception, otherwise null.
        /// </summary>
        public Exception Exception { get; set; }
    }
}