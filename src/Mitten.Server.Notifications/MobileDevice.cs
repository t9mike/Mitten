using System;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Contains details about an account's mobile device.
    /// </summary>
    public class MobileDevice
    {
        /// <summary>
        /// Initializes a new instance of the MobileDevice class.
        /// </summary>
        /// <param name="deviceId">A unique id for the device.</param>
        /// <param name="platformType">The type of platform used by the device.</param>
        /// <param name="appVersion">The version number for the application running on the device.</param>
        /// <param name="arePushNotificationsEnabled">A value indicating whether or not push notifications are enabled.</param>
        /// <param name="pushNotificationToken">A unique token identifying the device when sending push notifications.</param>
        public MobileDevice(
            string deviceId,
            MobileDevicePlatformType platformType, 
            string appVersion, 
            bool arePushNotificationsEnabled,
            string pushNotificationToken)
        {
            if (arePushNotificationsEnabled &&
                string.IsNullOrWhiteSpace(pushNotificationToken))
            {
                throw new ArgumentException("A push notification token must be specified if push notifications are enabled for the device.", nameof(pushNotificationToken));
            }

            this.DeviceId = deviceId;
            this.PlatformType = platformType;
            this.AppVersion = appVersion;
            this.ArePushNotificationsEnabled = arePushNotificationsEnabled;
            this.PushNotificationToken = pushNotificationToken;
        }

        /// <summary>
        /// Gets the unique id for the device.
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// Gets the type of platform used by the device.
        /// </summary>
        public MobileDevicePlatformType PlatformType { get; private set; }

        /// <summary>
        /// Gets the version number for the application running on the device.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        /// Gets whether or not push notifications for the current device are enabled.
        /// </summary>
        public bool ArePushNotificationsEnabled { get; private set; }

        /// <summary>
        /// Gets a unique token identifying the device when sending push notifications.
        /// </summary>
        public string PushNotificationToken { get; private set; }

        /// <summary>
        /// Disable push notifications for the current device.
        /// </summary>
        internal void DisablePushNotifications()
        {
            this.ArePushNotificationsEnabled = false;
        }

        /// <summary>
        /// Enables push notifications for the current device using the specified token.
        /// </summary>
        /// <param name="token">The notification token identifying the device.</param>
        internal void EnablePushNotifications(string token)
        {
            Throw.IfArgumentNullOrWhitespace(token, nameof(token));

            this.PushNotificationToken = token;
            this.ArePushNotificationsEnabled = true;
        }

        /// <summary>
        /// Sets the app version currently running on this device.
        /// </summary>
        /// <param name="appVersion">The app version.</param>
        internal void SetAppVersion(string appVersion)
        {
            Throw.IfArgumentNullOrWhitespace(appVersion, nameof(appVersion));
            this.AppVersion = appVersion;
        }
    }
}