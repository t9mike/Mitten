using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mitten.Server.Events;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// A channel for sending push notifications to client applications.
    /// </summary>
    public class PushNotificationChannel : NotificationChannel<PushNotification>
    {
        private readonly IMobilePushNotificationServiceClient mobileNotificationServiceClient;

        /// <summary>
        /// Initializes a new instance of the PushNotificationChannel class.
        /// </summary>
        /// <param name="mobileNotificationServiceClient">Mobile notification service client.</param>
        /// <param name="eventPublisher">Event publisher.</param>
        public PushNotificationChannel(IMobilePushNotificationServiceClient mobileNotificationServiceClient, IEventPublisher eventPublisher)
            : base(eventPublisher)
        {
            this.mobileNotificationServiceClient = mobileNotificationServiceClient;
        }

        /// <summary>
        /// Sends a notification to the specified account.
        /// </summary>
        /// <param name="account">An account to send the notification to.</param>
        /// <param name="notification">A notification to send.</param>
        /// <returns>The task for the operation.</returns>
        protected override Task DoSendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, PushNotification notification)
        {
            if (account.MobileDevices == null ||
                !account.MobileDevices.Any())
            {
                this.OnFailed(
                    notification,
                    NotificationErrorCode.MobileDeviceNotRegistered,
                    account.AccountId.ToString(),
                    "No mobile devices have been registered to receive the push notification.");

                return Task.FromResult(false);
            }

            return Task.WhenAll(this.SendToAllDevicesAsync(account, notification));
        }

        private IEnumerable<Task> SendToAllDevicesAsync<TAccountKey>(NotificationAccount<TAccountKey> account, PushNotification notification)
        {
            List<Task> tasks = new List<Task>();
            foreach (MobileDevice mobileDevice in account.MobileDevices)
            {
                if (this.ValidateDevice(account, mobileDevice, notification))
                {
                    tasks.Add(this.SendAsync(account, mobileDevice, notification));
                }
            }

            return tasks;
        }

        private async Task SendAsync<TAccountKey>(NotificationAccount<TAccountKey> account, MobileDevice mobileDevice, PushNotification notification)
        {
            PushNotificationResult result = await this.mobileNotificationServiceClient.SendNotification(mobileDevice, notification).ConfigureAwait(false);

            if (result.WasSuccessful)
            {
                this.OnSuccess(notification, account.AccountId.ToString(), mobileDevice.DeviceId, result.EndpointName);
            }
            else
            {
                this.OnFailed(
                    notification,
                    NotificationErrorCode.RouteFailure,
                    account.AccountId.ToString(),
                    result.ErrorMessage,
                    mobileDevice.DeviceId,
                    result.EndpointName);
            }
        }

        private bool ValidateDevice<TAccountKey>(NotificationAccount<TAccountKey> account, MobileDevice mobileDevice, PushNotification notification)
        {
            if (!mobileDevice.ArePushNotificationsEnabled)
            {
                this.OnFailed(
                    notification,
                    NotificationErrorCode.Disabled,
                    account.AccountId.ToString(),
                    "The notification could not be sent because push notifications on the destination device are not enabled.",
                    mobileDevice.DeviceId);

                return false; 
            }

            if (!this.ValidateAppVersion(mobileDevice, notification))
            {
                this.OnFailed(
                    notification,
                    NotificationErrorCode.VersionMismatch,
                    account.AccountId.ToString(),
                    "The app version on the mobile device does not support the notification.",
                    mobileDevice.DeviceId);

                return false;
            }

            return true;
        }

        private bool ValidateAppVersion(MobileDevice mobileDevice, PushNotification notification)
        {
            if (notification.MinimumAppVersion == null && notification.MaximumAppVersion == null)
            {
                return true;
            }

            Version appVersion;

            if (Version.TryParse(mobileDevice.AppVersion, out appVersion) &&
                appVersion >= notification.MinimumAppVersion)
            {
                return notification.MaximumAppVersion == null || appVersion <= notification.MaximumAppVersion;
            }

            return false;
        }
    }
}
