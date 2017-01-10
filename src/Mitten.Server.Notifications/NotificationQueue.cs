using System.Threading.Tasks;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Handles queuing notifications to be sent.
    /// </summary>
    public class NotificationQueue
    {
        private readonly INotificationChannelFactory channelFactory;

        /// <summary>
        /// Initializes a new instance of the NotificationQueue class.
        /// </summary>
        /// <param name="channelFactory">A notification channel factory.</param>
        public NotificationQueue(INotificationChannelFactory channelFactory)
        {
            Throw.IfArgumentNull(channelFactory, nameof(channelFactory));
            this.channelFactory = channelFactory;
        }

        /// <summary>
        /// Asynchronously sends a notification to the queue.
        /// </summary>
        /// <param name="account">The account to send the notification to.</param>
        /// <param name="notification">A notification to send.</param>
        /// <returns>The task.</returns>
        public Task SendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, Notification notification)
        {
            Throw.IfArgumentNull(account, nameof(account));
            Throw.IfArgumentNull(notification, nameof(notification));

            INotificationChannel channel = this.channelFactory.GetChannel(notification);
            return channel.SendAsync(account, notification);
        }
    }
}