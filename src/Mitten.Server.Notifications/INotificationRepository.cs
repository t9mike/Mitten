using System;
using System.Collections.Generic;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Defines a repository for storing scheduled notifications.
    /// </summary>
    /// <typeparam name="TKey">The type of key for the id of the account a notification should be sent to.</typeparam>
    public interface INotificationRepository<TKey>
    {
        /// <summary>
        /// Saves a notification to the repository.
        /// </summary>
        /// <param name="notification">A notification to save.</param>
        void SaveScheduledNotification(ScheduledNotification<TKey> notification);

        /// <summary>
        /// Gets a notification.
        /// </summary>
        /// <param name="notificationId">The id of the notification.</param>
        /// <returns>The notification or null if a notification with the id does not exist.</returns>
        ScheduledNotification<TKey> GetScheduledNotification(Guid notificationId);

        /// <summary>
        /// Gets a list of notifications scheduled to be delivered on or before the specified delivery date.
        /// </summary>
        /// <param name="deliveryDateAndTime">A delivery date and time.</param>
        /// <returns>A list of notifications scheduled to be delivered on or before the specified date and time.</returns>
        IEnumerable<ScheduledNotification<TKey>> GetScheduledNotifications(DateTime deliveryDateAndTime);

        /// <summary>
        /// Deletes a scheduled notification with the specified id.
        /// </summary>
        /// <param name="notificationId">The id of the notification to delete.</param>
        /// <returns>The notification that was deleted, otherwise null if a notification did not exist.</returns>
        ScheduledNotification<TKey> DeleteScheduledNotification(Guid notificationId);
    }
}