using System.Threading.Tasks;
using Foundation;
using Mitten.Mobile.Application.PushNotifications;
using Mitten.Mobile.Identity;
using Mitten.Mobile.Remote;
using Mitten.Mobile.System;
using UIKit;

namespace Mitten.Mobile.iOS.Application.PushNotifications
{
    /// <summary>
    /// Handles push notification support for an iOS application.
    /// </summary>
    public class iOSPushNotificationRegistry : PushNotificationRegistry
    {
        private static class Constants
        {
            public const string NotificationTokenKey = "DeviceNotificationToken";
        }

        /// <summary>
        /// Initializes a new instance of the iOSPushNotificationRegistry class.
        /// </summary>
        /// <param name="notificationServerRegistration">Handlers registering push notification tokens with a remote server.</param>
        /// <param name="systemInformation">An object that provides information about the current system.</param>
        public iOSPushNotificationRegistry(IPushNotificationServerRegistration notificationServerRegistration, ISystemInformation systemInformation)
            : base(notificationServerRegistration, systemInformation)
        {
        }

        /// <summary>
        /// Gets whether or not remote notifications are enabled for the current device.
        /// </summary>
        public override bool AreRemoteNotificationsEnabled
        {
            get
            {
                UIUserNotificationSettings settings = UIApplication.SharedApplication.CurrentUserNotificationSettings;
                return (settings.Types & UIUserNotificationType.Alert) == UIUserNotificationType.Alert;
            }
        }

        /// <summary>
        /// Gets the type of platform this registry supports.
        /// </summary>
        protected override PlatformType PlatformType
        {
            get { return PlatformType.iOS; }
        }

        /// <summary>
        /// Requests a notification token identifying this device.
        /// </summary>
        public override void RequestNotificationToken()
        {
            UIUserNotificationSettings pushSettings =
                UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        /// <summary>
        /// Update the notification token for the specified account.
        /// </summary>
        /// <param name="account">The account for the current user.</param>
        /// <param name="notificationToken">A notification token uniquely identifying this device for sending push notifications.</param>
        /// <returns>The task for the operation.</returns>
        public override Task<ServiceResult> UpdateNotificationToken(IAccount account, string notificationToken)
        {
            Throw.IfArgumentNullOrWhitespace(notificationToken, "notificationToken");

            notificationToken = notificationToken.Trim('<', '>').Replace(" ", string.Empty);
            return this.UploadDeviceNotificationToken(account, notificationToken);
        }
    }
}