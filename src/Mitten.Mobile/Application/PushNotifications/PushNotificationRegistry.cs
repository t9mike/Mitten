using System.Threading.Tasks;
using Mitten.Mobile.Identity;
using Mitten.Mobile.Remote;
using Mitten.Mobile.System;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Handles enabling push notifications for a device.
    /// </summary>
    public abstract class PushNotificationRegistry
    {
        private readonly ISystemInformation systemInformation;
        private readonly IPushNotificationServerRegistration notificationServerRegistration;

        /// <summary>
        /// Initializes a new instance of the PushNotificationRegistry class.
        /// </summary>
        /// <param name="notificationServerRegistration">Handlers registering push notification tokens with a remote server.</param>
        /// <param name="systemInformation">An object that provides information about the current system.</param>
        protected PushNotificationRegistry(IPushNotificationServerRegistration notificationServerRegistration, ISystemInformation systemInformation)
        {
            Throw.IfArgumentNull(notificationServerRegistration, nameof(notificationServerRegistration));
            Throw.IfArgumentNull(systemInformation, nameof(systemInformation));

            this.notificationServerRegistration = notificationServerRegistration;
            this.systemInformation = systemInformation;
        }

        /// <summary>
        /// Gets whether or not remote notifications are enabled for the current device.
        /// </summary>
        public abstract bool AreRemoteNotificationsEnabled { get; }

        /// <summary>
        /// Gets the type of platform this registry supports.
        /// </summary>
        protected abstract PlatformType PlatformType { get; }

        /// <summary>
        /// Sends a request to the push notification service for a token identifying the current device.
        /// </summary>
        public abstract void RequestNotificationToken();

        /// <summary>
        /// Update the notification token for the specified account.
        /// </summary>
        /// <param name="account">The account for the current user.</param>
        /// <param name="notificationToken">A notification token uniquely identifying this device for sending push notifications.</param>
        /// <returns>The task for the operation.</returns>
        public abstract Task<ServiceResult> UpdateNotificationToken(IAccount account, string notificationToken);

        /// <summary>
        /// Uploads the notification token for the current device.
        /// </summary>
        /// <param name="account">A user account.</param>
        /// <param name="notificationToken">A notification token uniquely identifying this device for sending push notifications.</param>
        /// <returns>The task for the operation.</returns>
        protected Task<ServiceResult> UploadDeviceNotificationToken(IAccount account, string notificationToken)
        {
            return
                this.notificationServerRegistration.RegisterNotificationToken(
                    account,
                    this.PlatformType,
                    this.systemInformation.GetAppVersion(),
                    notificationToken);
        }
    }
}
