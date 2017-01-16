using Mitten.Server.Events;

namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Base class for events raised inside the notifications component.
    /// </summary>
    public abstract class NotificationEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of the NotificationEvent class.
        /// </summary>
        /// <param name="notification">The notification for the event.</param>
        internal NotificationEvent(Notification notification)
        {
            this.Notification = notification;
        }

        /// <summary>
        /// Gets the notification for the event.
        /// </summary>
        public Notification Notification { get; }
    }
}
