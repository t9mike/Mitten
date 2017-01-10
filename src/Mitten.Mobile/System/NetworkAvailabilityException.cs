using System;

namespace Mitten.Mobile.System
{
    /// <summary>
    /// Represents an error caused by an invalid or unsupported network availability.
    /// </summary>
    public class NetworkAvailabilityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NetworkAvailabilityException class.
        /// </summary>
        /// <param name="currentAvailability">The current availability.</param>
        internal NetworkAvailabilityException(NetworkAvailability currentAvailability)
            : base("The current network availability (" + currentAvailability + ") is invalid for the request.")
        {
        }
    }
}