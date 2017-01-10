using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents an error due to a bad command request caused by invalid input arguments or state.
    /// Note: a bad request exception will not cause any additional failure recovery logic.
    /// </summary>
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the BadRequestException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public BadRequestException(string message)
            : this(null, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BadRequestException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        public BadRequestException(string errorCode, string message)
            : this (errorCode, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BadRequestException class.
        /// </summary>
        /// <param name="errorCode">A code identifying the specific exception.</param>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">An exception that caused the current error.</param>
        public BadRequestException(string errorCode, string message, Exception innerException)
            : base (message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets a code that uniquely identifies the specific issue.
        /// </summary>
        public string ErrorCode { get; private set; }
    }
}