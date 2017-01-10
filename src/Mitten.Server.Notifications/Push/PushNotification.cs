using System;
using System.Collections.Generic;
using System.Linq;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// Represents a notification that should be sent to an application.
    /// </summary>
    public class PushNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the PushNotification class.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        /// <param name="alertText">The alert text for the notification.</param>
        /// <param name="content">The content for the push notification.</param>
        /// <param name="minimumAppVersion">The minimum supported mobile app version for the notification or null if the notification is supported by all versions.</param>
        /// <param name="maximumAppVersion">The maximum supported mobile app version for the notification or null if the notification is supported by all versions after the minimum.</param>
        public PushNotification(
            string name,
            string alertText,
            IEnumerable<KeyValuePair<string, string>> content,
            Version minimumAppVersion = null,
            Version maximumAppVersion = null)
            : base(name, NotificationType.Push)
        {
            Throw.IfArgumentNullOrWhitespace(alertText, nameof(alertText));

            if (minimumAppVersion == null && maximumAppVersion != null)
            {
                throw new ArgumentNullException(nameof(minimumAppVersion), "A minimum app version must be specified if the maximum app version is not null.");
            }

            this.AlertText = alertText;
            this.Content = content ?? Enumerable.Empty<KeyValuePair<string, string>>();
            this.MinimumAppVersion = minimumAppVersion;
            this.MaximumAppVersion = maximumAppVersion;
        }

        /// <summary>
        /// Gets the text that is displayed to the user when receiving the notification.
        /// </summary>
        public string AlertText { get; private set; }

        /// <summary>
        /// Gets the content for the push notification.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Content { get; private set; }

        /// <summary>
        /// Gets the minimum supported mobile app version for the notification or null if the notification is supported by all versions.
        /// </summary>
        public Version MinimumAppVersion { get; private set; }

        /// <summary>
        /// Gets the maximum supported mobile app version for the notification or null if the notification is supported by all versions after the minimum.
        /// </summary>
        public Version MaximumAppVersion { get; private set; }
    }
}
