using System;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Represents an error due to a session that has expired.
    /// </summary>
    public class SessionExpiredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SessionExpiredException class.
        /// </summary>
        internal SessionExpiredException()
            : base("The current session has expired.")
        {
        }
    }
}
