namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Occurs when a scheduled notification has been canceled.
    /// </summary>
    public class ScheduledNotificationCanceled : NotificationEvent
    {
        /// <summary>
        /// Initializes a new instance of the NotificationCanceled class.
        /// </summary>
        /// <param name="notification">The notification that was canceled.</param>
        internal ScheduledNotificationCanceled(Notification notification)
            : base(notification)
        {
        }
    }
}
