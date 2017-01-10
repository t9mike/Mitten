using System;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Represents an error due to a notification account that could not be found.
    /// </summary>
    public class NotificationAccountNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NotificationAccountNotFoundException class.
        /// </summary>
        internal NotificationAccountNotFoundException(object accountId)
            : base ("A notification account with id (" + accountId + ") could not be found.")
        {
        }
    }
}