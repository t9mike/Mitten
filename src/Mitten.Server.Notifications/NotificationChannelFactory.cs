using System;
using System.Collections.Generic;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// A basic notification channel factory that returns channels based on the notification type.
    /// </summary>
    public class NotificationChannelFactory : INotificationChannelFactory
    {
        private readonly Dictionary<Type, Lazy<INotificationChannel>> channels;

        /// <summary>
        /// Initializes a new instance of the NotificationChannelFactory class.
        /// </summary>
        public NotificationChannelFactory()
        {
            this.channels = new Dictionary<Type, Lazy<INotificationChannel>>();
        }

        /// <summary>
        /// Registers a channel for a specific type of notification.
        /// </summary>
        /// <param name="channel">A channel to register.</param>
        public void RegisterChannel<TNotification>(INotificationChannel channel)
            where TNotification : Notification
        {
            this.RegisterChannel<TNotification>(() => channel);
        }

        /// <summary>
        /// Registers a factory method responsible for creating a new channel for a specific type of notification.
        /// </summary>
        /// <param name="createFactory">A factory method.</param>
        public void RegisterChannel<TNotification>(Func<INotificationChannel> createFactory)
            where TNotification : Notification
        {
            Throw.IfArgumentNull(createFactory, nameof(createFactory));

            Type notificationType = typeof(TNotification);
            if (this.channels.ContainsKey(notificationType))
            {
                throw new ArgumentException("A channel factory method is already registered for Type (" + notificationType.FullName + ").");
            }

            this.channels.Add(notificationType, new Lazy<INotificationChannel>(createFactory));
        }

        /// <summary>
        /// Gets a channel to use for sending the specified notification.
        /// </summary>
        /// <param name="notification">A notification to get the channel for.</param>
        /// <returns>The channel.</returns>
        public INotificationChannel GetChannel(Notification notification)
        {
            Lazy<INotificationChannel> factoryMethod;
            if (!this.channels.TryGetValue(notification.GetType(), out factoryMethod))
            {
                throw new ArgumentException("A channel factory method has not been registered for Type (" + notification.GetType().FullName + ").", nameof(notification));
            }

            return factoryMethod.Value;
        }
    }
}
