using System;
using System.Collections.Generic;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Base class for objects that contain information received from a push notification.
    /// </summary>
    public abstract class PushNotification
    {
        private static class Constants
        {
            public const string NotificationNameKey = "name";
        }

        /// <summary>
        /// Initializes a new instance of the PushNotification class.
        /// </summary>
        protected PushNotification()
        {
        }

        /// <summary>
        /// Attempts to get a value from the dictionary based on a provided key and returns null if no value exists.
        /// </summary>
        /// <param name="values">A collection of values.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value from the collection, or null if no value was found for the given key.</returns>
        protected string TryGetValue(IDictionary<string, string> values, string key)
        {
            string value;

            return 
                values.TryGetValue(key, out value) 
                ? value 
                : null;

        }

        /// <summary>
        /// Attempts to parse the specified value.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="onParsed">A callback that is invoked if the value was successfully parsed.</param>
        /// <returns>True if the value was successfully parsed, otherwise false.</returns>
        protected bool TryParseGuid(string input, Action<Guid> onParsed)
        {
            return this.TryParseValue(input, Guid.TryParse, onParsed);
        }

        /// <summary>
        /// Attempts to parse the specified enum value.
        /// </summary>
        /// <typeparam name="TEnum">The Type of enum being parsed.</typeparam>
        /// <param name="input">The input to parse.</param>
        /// <param name="onParsed">A callback that is invoked if the value was successfully parsed.</param>
        /// <returns>True if the value was successfully parsed, otherwise false.</returns>
        protected bool TryParseEnum<TEnum>(string input, Action<TEnum> onParsed)
            where TEnum : struct
        {
            return this.TryParseValue(input, Enum.TryParse, onParsed);
        }

        /// <summary>
        /// Initializes the notification from the given list of key/value pairs received from the push notification server.
        /// </summary>
        /// <param name="values">A dictionary of key/value pairs.</param>
        /// <returns>True if the notification was successfully initialized and valid, otherwise false.</returns>
        protected abstract bool Initialize(IDictionary<string, string> values);

        /// <summary>
        /// Creates a new push notification from a collection of key/value pairs or null if no notification Type is defined.
        /// </summary>
        /// <param name="knownTypes">A collection of known notifiation types.</param>
        /// <param name="notificationValues">A key/value pair of items for the notification.</param>
        /// <returns>The result of the conversion operation.</returns>
        public static PushNotificationParseResult FromDictionary(PushNotificationTypes knownTypes, IDictionary<string, string> notificationValues)
        {
            Throw.IfArgumentNull(notificationValues, "notificationValues");

            Dictionary<string, string> values = new Dictionary<string, string>(notificationValues);

            if (values.Comparer != StringComparer.OrdinalIgnoreCase)
            {
                values = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            }

            if (!values.ContainsKey(Constants.NotificationNameKey))
            {
                return new PushNotificationParseResult(PushNotificationParseResult.ResultCode.MissingName);
            }

            string name = values[Constants.NotificationNameKey];
            Type notificationType = knownTypes.TryGetType(name);

            if (notificationType != null)
            {
                PushNotification notification = (PushNotification)Activator.CreateInstance(notificationType);

                return 
                    notification.Initialize(values)
                    ? new PushNotificationParseResult(notification)
                    : new PushNotificationParseResult(PushNotificationParseResult.ResultCode.FailedToInitialize);
            }

            return new PushNotificationParseResult(PushNotificationParseResult.ResultCode.UnknownName);
        }

        private delegate bool TryParse<TValue>(string input, out TValue value) where TValue : struct; 
        private bool TryParseValue<TValue>(string input, TryParse<TValue> tryParse, Action<TValue> onParsed)
            where TValue : struct
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                TValue value;
                if (tryParse(input, out value))
                {
                    onParsed(value);
                    return true;
                }
            }

            return false;
        }
    }
}