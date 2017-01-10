using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mitten.Server.Events;
using Mitten.Server.Extensions;
using Mitten.Server.Notifications.Events;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Handles scheduling notifications to be sent at a later date.
    /// </summary>
    /// <remarks>
    /// Currently there is no delivery guarantee of a scheduled notification, the scheduler
    /// will only attempt to send the notification once regardless of the outcome.
    /// </remarks>
    public class NotificationScheduler<TKey>
        where TKey : struct
    {
        private static class Constants
        {
            public const string EasternTimeZone = "US/Eastern";
        }

        private readonly NotificationQueue notificationQueue;
        private readonly NotificationAccountManager<TKey> accountManager;
        private readonly INotificationRepository<TKey> repository;
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Initializes a new instance of the NotificationScheduler class.
        /// </summary>
        /// <param name="notificationQueue">A notification queue used to send the notifications.</param>
        /// <param name="accountManager">A notification account manager.</param>
        /// <param name="repository">A repository for storing scheduled notifications.</param>
        /// <param name="eventPublisher">An event publisher.</param>
        public NotificationScheduler(
            NotificationQueue notificationQueue,
            NotificationAccountManager<TKey> accountManager,
            INotificationRepository<TKey> repository,
            IEventPublisher eventPublisher)
        {
            Throw.IfArgumentNull(notificationQueue, nameof(notificationQueue));
            Throw.IfArgumentNull(accountManager, nameof(accountManager));
            Throw.IfArgumentNull(repository, nameof(repository));
            Throw.IfArgumentNull(eventPublisher, nameof(eventPublisher));

            this.notificationQueue = notificationQueue;
            this.accountManager = accountManager;
            this.repository = repository;
            this.eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Attempts to schedule a notification for delivery.
        /// </summary>
        /// <param name="accountId">The id of the account the notification will be sent to.</param>
        /// <param name="notification">A notification to schedule.</param>
        /// <param name="deliveryDateTime">The date and time the notification should be delivered.</param>
        /// <returns>True if the notification was scheduled or false if an account to send the notification to does not exist.</returns>
        public bool TrySchedule(TKey accountId, Notification notification, DateTime deliveryDateTime)
        {
            NotificationAccount<TKey> account = this.accountManager.TryGetAccount(accountId);

            if (account != null)
            {
                this.Schedule(account, notification, deliveryDateTime);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Schedules a notification for delivery.
        /// </summary>
        /// <param name="accountId">The id of the account the notification will be sent to.</param>
        /// <param name="notification">A notification to schedule.</param>
        /// <param name="deliveryDateTime">The date and time the notification should be delivered.</param>
        public void Schedule(TKey accountId, Notification notification, DateTime deliveryDateTime)
        {
            this.Schedule(this.accountManager.GetAccount(accountId), notification, deliveryDateTime);
        }

        /// <summary>
        /// Schedules a notification for delivery.
        /// </summary>
        /// <param name="account">The account the notification will be sent to.</param>
        /// <param name="notification">A notification to schedule.</param>
        /// <param name="deliveryDateTime">The date and time the notification should be delivered.</param>
        public void Schedule(NotificationAccount<TKey> account, Notification notification, DateTime deliveryDateTime)
        {
            Throw.IfArgumentNull(account, nameof(account));
            Throw.IfArgumentNull(notification, nameof(notification));

            string timeZone =
                string.IsNullOrWhiteSpace(account.TimeZone)
                ? Constants.EasternTimeZone
                : account.TimeZone;

            ScheduledNotification<TKey> scheduledNotification =
                new ScheduledNotification<TKey>(
                    account.AccountId,
                    notification,
                    deliveryDateTime.ToDateTimeOffset(timeZone));

            this.repository.SaveScheduledNotification(scheduledNotification);

            this.eventPublisher.Publish(
                new NotificationScheduled(
                    scheduledNotification.AccountId.ToString(),
                    scheduledNotification.Notification,
                    scheduledNotification.DeliveryDateTime));
        }

        /// <summary>
        /// Sends any scheduled notifications that are over due for delivery.
        /// </summary>
        /// <param name="deliveryDateTime">The delivery date for the scheduled notifications. All notifications scheduled for deliver on or before this date will be sent.</param>
        /// <returns>The task for the operation.</returns>
        public Task SendScheduledNotifications(DateTime deliveryDateTime)
        {
            return this.SendScheduledNotifications(this.repository.GetScheduledNotifications(deliveryDateTime));
        }

        /// <summary>
        /// Cancels a scheduled notification with the specified id.
        /// </summary>
        /// <param name="notificationId">The id of the notification to cancel.</param>
        /// <returns>True if the scheduled notification was canceled, otherwise false if the notification was not currently scheduled.</returns>
        public bool Cancel(Guid notificationId)
        {
            ScheduledNotification<TKey> notification = this.repository.DeleteScheduledNotification(notificationId);
            if (notification != null)
            {
                this.eventPublisher.Publish(new ScheduledNotificationCanceled(notification.Notification));
                return true;
            }

            return false;
        }

        private async Task SendScheduledNotifications(IEnumerable<ScheduledNotification<TKey>> scheduledNotificationsToSend)
        {
            foreach (ScheduledNotification<TKey> scheduledNotification in scheduledNotificationsToSend)
            {
                NotificationAccount<TKey> account = this.accountManager.TryGetAccount(scheduledNotification.AccountId);

                if (account != null)
                {
                    await this.notificationQueue.SendAsync(account, scheduledNotification.Notification).ConfigureAwait(false);
                }
                else
                {
                    this.eventPublisher.Publish(
                        new NotificationSendFailure(
                            scheduledNotification.Notification,
                            NotificationErrorCode.AccountNotFound,
                            scheduledNotification.AccountId.ToString(),
                            "An account could not be found for the scheduled notification."));

                }

                this.repository.DeleteScheduledNotification(scheduledNotification.Notification.Id);
            }
        }
    }
}