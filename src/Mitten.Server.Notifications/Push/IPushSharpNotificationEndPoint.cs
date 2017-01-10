using System.Threading.Tasks;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// Represents a platform specific end-point for sending notifications using PushSharp.
    /// </summary>
    public interface IPushSharpNotificationEndPoint
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
