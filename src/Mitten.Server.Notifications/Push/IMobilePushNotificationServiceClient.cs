using System.Threading.Tasks;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// Defines a client that handles connecting to a service for sending push notifications to mobile devices.
    /// </summary>
    public interface IMobilePushNotificationServiceClient
    {
        /// <summary>
        /// Sends the specified push notification.
        /// </summary>
        /// <param name="mobileDevice">The mobile device to send the notification to.</param>
        /// <param name="notification">The notification to send.</param>
        /// <returns>The task and result of the operation.</returns>
        Task<PushNotificationResult> SendNotification(MobileDevice mobileDevice, PushNotification notification);
    }
}