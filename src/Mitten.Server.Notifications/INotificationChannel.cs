using System.Threading.Tasks;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Represents a communication channel for sending a notification.
    /// </summary>
    public interface INotificationChannel
    {
        /// <summary>
        /// Sends a notification to the specified account.
        /// </summary>
        /// <param name="account">An account to send the notification to.</param>
        /// <param name="notification">A notification to send.</param>
        /// <returns>The task for the operation.</returns>
        Task SendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, Notification notification);
    }
}