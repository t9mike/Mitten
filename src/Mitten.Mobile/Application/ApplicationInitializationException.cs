using System;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Represents an error that occured while initializing a new application instance.
    /// </summary>
    public class ApplicationInitializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationInitializationException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public ApplicationInitializationException(string message)
            : base(message)
        {
        }
    }
}
