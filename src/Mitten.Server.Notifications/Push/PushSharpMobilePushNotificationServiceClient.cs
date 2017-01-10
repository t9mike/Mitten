using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// A push notification client that uses PushSharp end-points for service connections.
    /// </summary>
    public class PushSharpMobilePushNotificationServiceClient : IMobilePushNotificationServiceClient
    {
        private readonly Dictionary<MobileDevicePlatformType, IPushSharpNotificationEndPoint> notificationEndPoints;

        /// <summary>
        /// Initializes a new instance of the PushSharpMobilePushNotificationServiceClient class.
        /// </summary>
        public PushSharpMobilePushNotificationServiceClient()
        {
            this.notificationEndPoints = new Dictionary<MobileDevicePlatformType, IPushSharpNotificationEndPoint>();
        }
        
        /// <summary>
        /// Registers an end-point for notifications based on a mobile device platform.
        /// </summary>
        /// <param name="platformType">The device platform.</param>
        /// <param name="notificationEndPoint">The end-point for the specific platform.</param>
        public void RegisterEndPoint(MobileDevicePlatformType platformType, IPushSharpNotificationEndPoint notificationEndPoint)
        {
            if (this.notificationEndPoints.ContainsKey(platformType))
            {
                throw new ArgumentException("Platform type (" + platformType + ") has already been registered.", nameof(platformType));
            }
            
            this.notificationEndPoints.Add(platformType, notificationEndPoint);
        }

        /// <summary>
        /// Sends the specified push notification.
        /// </summary>
        /// <param name="mobileDevice">The mobile device to send the notification to.</param>
        /// <param name="notification">The notification to send.</param>
        /// <returns>The task and result of the operation.</returns>
        public Task<PushNotificationResult> SendNotification(MobileDevice mobileDevice, PushNotification notification)
        {
            Throw.IfArgumentNull(notification, "notification");
            return this.GetEndPoint(mobileDevice).SendNotification(mobileDevice, notification);
        }

        private IPushSharpNotificationEndPoint GetEndPoint(MobileDevice mobileDevice)
        {
            if (!this.notificationEndPoints.ContainsKey(mobileDevice.PlatformType))
            {
                throw new ArgumentException("No end-point has been registered for the mobile device platform (" + mobileDevice.PlatformType + ").", nameof(mobileDevice));
            }

            return this.notificationEndPoints[mobileDevice.PlatformType];
        }
    }
}