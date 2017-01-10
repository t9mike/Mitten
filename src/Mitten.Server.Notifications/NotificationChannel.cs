using System;
using System.Threading.Tasks;
using Mitten.Server.Events;
using Mitten.Server.Notifications.Events;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Base class for notification channels.
    /// </summary>
    public abstract class NotificationChannel<TNotification> : INotificationChannel
        where TNotification : Notification
    {
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Initializes a new instance of the NotificationChannel class.
        /// </summary>
        /// <param name="eventPublisher">A publisher used to notification raise events that occur in this channel.</param>
        protected NotificationChannel(IEventPublisher eventPublisher)
        {
            Throw.IfArgumentNull(eventPublisher, nameof(eventPublisher));
            this.eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Sends a notification to the specified account.
        /// </summary>
        /// <param name="account">An account to send the notification to.</param>
        /// <param name="notification">A notification to send.</param>
        /// <returns>The task for the operation.</returns>
        public async Task SendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, Notification notification)
        {
            try
            {
                await this.DoSendAsync(account, (TNotification)notification).ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                this.OnError(account.AccountId.ToString(), notification, ex);
            }
        }

        /// <summary>
        /// Sends a notification to the specified account.
        /// </summary>
        /// <param name="account">An account to send the notification to.</param>
        /// <param name="notification">A notification to send.</param>
        /// <returns>The task for the operation.</returns>
        protected abstract Task DoSendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, TNotification notification);

        /// <summary>
        /// Publishes a notification event.
        /// </summary>
        /// <param name="eventData">The event data to publish.</param>
        protected void Publish(NotificationEvent eventData)
        {
            this.eventPublisher.Publish(eventData);
        }

        /// <summary>
        /// Occurs when a notification has been sent successfully.
        /// </summary>
        /// <param name="notification">The inotification that has been sent.</param>
        /// <param name="accountId">The id of the account that the notification was sent to.</param>
        /// <param name="destination">The id of the destination for the notification.</param>
        /// <param name="endpointName">The endpoint the notification was intended for.</param>
        protected void OnSuccess(Notification notification, string accountId, string destination, string endpointName)
        {
            this.eventPublisher.Publish(new NotificationSent(notification, accountId, destination, endpointName));
        }

        /// <summary>
        /// Occurs when a notification failed to send.
        /// </summary>
        /// <param name="notification">The notification that failed to send.</param>
        /// <param name="errorCode">The error code for the notification.</param>
        /// <param name="accountId">The id of the account the notification was intended for.</param>
        /// <param name="description">A description of the failure.</param>
        /// <param name="destination">The id of the destination for the notification.</param>
        /// <param name="endpointName">The endpoint the notification was intended for.</param>
        protected void OnFailed(
            Notification notification,
            NotificationErrorCode errorCode,
            string accountId,
            string description,
            string destination = null,
            string endpointName = null)
        {
            this.eventPublisher.Publish(
                new NotificationSendFailure(
                    notification,
                    errorCode,
                    accountId,
                    description,
                    destination,
                    endpointName));
        }

        private void OnError(string accountId, Notification notification, Exception ex)
        {
            this.Publish(new NotificationInternalError(notification, accountId, "An error occurred while trying to send notification.", ex));
        }
    }
}
