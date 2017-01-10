using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents a request error for a resource that could not be found based on the provided inputs.
    /// </summary>
    public class ResourceNotFoundException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public ResourceNotFoundException(string message)
            : this(null, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        public ResourceNotFoundException(string errorCode, string message)
            : this (errorCode, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">An exception that caused the current error.</param>
        public ResourceNotFoundException(string errorCode, string message, Exception innerException)
            : base (errorCode, message, innerException)
        {
        }
    }
}