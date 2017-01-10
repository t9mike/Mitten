using System;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Represents an error due to a mobile device not existing for a notification account.
    /// </summary>
    public class MobileDeviceNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MobileDeviceNotRegisteredException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        internal MobileDeviceNotRegisteredException(string message)
            : base(message)
        {
        }
    }
}
