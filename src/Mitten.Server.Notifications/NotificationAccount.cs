using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Contains details about an account registered for notifications.
    /// </summary>
    /// <typeparam name="TKey">Identifies the data type for the account's id.</typeparam>
    public class NotificationAccount<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the NotificationAccount class.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        public NotificationAccount(TKey accountId)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        /// Initializes a new instance of the NotificationAccount class.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        /// <param name="email">An email associated with the account.</param>
        /// <param name="name">A name associated with the account.</param>
        /// <param name="timeZone">A time zone associated with the account.</param>
        /// <param name="mobileDevices">An optional set of mobile devices for the account.</param>
        [JsonConstructor]
        public NotificationAccount(
            TKey accountId, 
            string email, 
            string name,
            string timeZone,
            IEnumerable<MobileDevice> mobileDevices)
        {
            this.AccountId = accountId;
            this.Email = email;
            this.Name = name;
            this.TimeZone = timeZone;
            this.MobileDevices = mobileDevices;
        }

        /// <summary>
        /// Gets the account id for the recipient.
        /// </summary>
        public TKey AccountId { get; private set; }

        /// <summary>
        /// Gets or sets the email associated with the account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time zone associated with the account.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets the mobile devices associated with the account.
        /// </summary>
        public IEnumerable<MobileDevice> MobileDevices { get; private set; }

        /// <summary>
        /// Adds a new mobile device to the current account.
        /// </summary>
        /// <param name="mobileDevice">A mobile device.</param>
        internal void AddMobileDevice(MobileDevice mobileDevice)
        {
            Throw.IfArgumentNull(mobileDevice, nameof(mobileDevice));

            List<MobileDevice> mobileDevices = new List<MobileDevice>(new[] { mobileDevice });

            if (this.MobileDevices != null)
            {
                mobileDevices.AddRange(this.MobileDevices);
            }

            this.MobileDevices = mobileDevices;
        }

        /// <summary>
        /// Enables push notifications for a device associated with this account.
        /// </summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="pushNotificationToken">A push notification token.</param>
        internal void EnablePushNotifications(string deviceId, string pushNotificationToken)
        {
            Throw.IfArgumentNullOrWhitespace(deviceId, nameof(deviceId));
            Throw.IfArgumentNullOrWhitespace(pushNotificationToken, nameof(pushNotificationToken));

            MobileDevice device = this.GetDevice(deviceId);
            device.EnablePushNotifications(pushNotificationToken);
        }

        /// <summary>
        /// Disables push notifications for all the devices registered with this account.
        /// </summary>
        internal void DisablePushNotifications()
        {
            if (this.MobileDevices != null)
            {
                foreach (MobileDevice device in this.MobileDevices)
                {
                    device.DisablePushNotifications();
                }
            }
        }

        private MobileDevice GetDevice(string deviceId)
        {
            if (this.MobileDevices != null)
            {
                foreach (MobileDevice device in this.MobileDevices)
                {
                    if (device.DeviceId == deviceId)
                    {
                        return device;
                    }
                }
            }

            throw new MobileDeviceNotRegisteredException("A mobile device with id (" + deviceId + ") has not been registered with the current account.");
        }
    }
}
