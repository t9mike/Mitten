using System;
using Mitten.Server.Events;

namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Occurs when a notification has been scheduled.
    /// </summary>
    public class NotificationScheduled : EventBase
    {
        /// <summary>
        /// Initializes a new instance of the NotificationScheduled class.
        /// </summary>
        /// <param name="accountId">The id of the account that the notification will be sent to.</param>
        /// <param name="notification">The notification that was scheduled.</param>
        /// <param name="deliveryDateTime">The date and time the notification should be delivered.</param>
        internal NotificationScheduled(string accountId, Notification notification, DateTimeOffset deliveryDateTime)
        {
            Throw.IfArgumentNullOrWhitespace(accountId, nameof(accountId));
            Throw.IfArgumentNull(notification, nameof(notification));

            this.AccountId = accountId;
            this.Notification = notification;
            this.DeliveryDateTime = deliveryDateTime;
        }

        /// <summary>
        /// Gets the id of the account the notification is scheduled for.
        /// </summary>
        public string AccountId { get; private set; }

        /// <summary>
        /// Gets the notification that was scheduled.
        /// </summary>
        public Notification Notification { get; private set; }

        /// <summary>
        /// Gets the date and time the notification should be delivered.
        /// </summary>
        public DateTimeOffset DeliveryDateTime { get; private set; }
    }
}
