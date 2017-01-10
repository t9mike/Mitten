using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents an error due to an invalid state somewhere in the system.
    /// </summary>
    public class InvalidStateException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidStateException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public InvalidStateException(string message)
            : this(null, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidStateException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        public InvalidStateException(string errorCode, string message)
            : this (errorCode, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidStateException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">An exception that caused the current error.</param>
        public InvalidStateException(string errorCode, string message, Exception innerException)
            : base (errorCode, message, innerException)
        {
        }
    }
}