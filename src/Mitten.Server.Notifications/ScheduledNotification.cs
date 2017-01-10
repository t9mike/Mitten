using System;
using Mitten.Server.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Represents a notification that should be scheduled for delivery at a later date and time.
    /// </summary>
    public class ScheduledNotification<TKey> : IJsonSerializable
    {
        /// <summary>
        /// Initializes a new instance of the ScheduledNotification class.
        /// </summary>
        /// <param name="accountId">The id of the account the notification will be sent to.</param>
        /// <param name="notification">The notification that is scheduled for delivery.</param>
        /// <param name="deliveryDateTime">The date and time the notification should be delivered.</param>
        internal ScheduledNotification(TKey accountId, Notification notification, DateTimeOffset deliveryDateTime)
        {
            Throw.IfArgumentNull(notification, nameof(notification));

            this.AccountId = accountId;
            this.Notification = notification;
            this.DeliveryDateTime = deliveryDateTime;
        }

        /// <summary>
        /// Gets the id of the account the notification will be sent to.
        /// </summary>
        public TKey AccountId { get; private set; }

        /// <summary>
        /// Gets the notification that is scheduled for delivery.
        /// </summary>
        public Notification Notification { get; private set; }

        /// <summary>
        /// Gets the date and time the notification should be delivered.
        /// </summary>
        public DateTimeOffset DeliveryDateTime { get; private set; }

        /// <summary>
        /// Converts the current notification into a json string.
        /// </summary>
        /// <returns>A json string.</returns>
        public string ToJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.Formatting = Formatting.None;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.TypeNameHandling = TypeNameHandling.All;

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new VersionConverter());

            return JsonConvert.SerializeObject(this, settings);
        }

        /// <summary>
        /// Converts the specified json string into a notification instance.
        /// </summary>
        /// <param name="json">A json string.</param>
        /// <returns>A notification.</returns>
        public static ScheduledNotification<TKey> FromJson(string json)
        {
            Throw.IfArgumentNullOrWhitespace(json, nameof(json));

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings.ContractResolver = new PrivateSetterContractResolver();
            settings.TypeNameHandling = TypeNameHandling.All;

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new NotificationJsonConverter());
            settings.Converters.Add(new VersionConverter());

            return (ScheduledNotification<TKey>)JsonConvert.DeserializeObject(json, settings);
        }
    }
}