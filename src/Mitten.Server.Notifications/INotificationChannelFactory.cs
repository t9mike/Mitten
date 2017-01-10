namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Handles creating notification channels for specific notifications.
    /// </summary>
    public interface INotificationChannelFactory
    {
        /// <summary>
        /// Gets a channel to use for sending the specified notification.
        /// </summary>
        /// <param name="notification">A notification to get the channel for.</param>
        /// <returns>The channel.</returns>
        INotificationChannel GetChannel(Notification notification);
    }
}