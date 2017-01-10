using System;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Specifies that an object contains information about a received push notification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class PushNotificationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the PushNotificationAttribute class.
        /// </summary>
        /// <param name="notificationName">The name identifying the push notification the class represents.</param>
        public PushNotificationAttribute(string notificationName)
        {
            Throw.IfArgumentNullOrWhitespace(notificationName, "notificationName");
            this.NotificationName = notificationName;
        }

        /// <summary>
        /// Gets the name identifying the push notification the class represents
        /// </summary>
        public string NotificationName { get; private set; }
    }
}