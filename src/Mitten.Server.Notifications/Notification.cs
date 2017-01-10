using System;
using System.Collections.Generic;
using System.Linq;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Base class for a notification.
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// Initializes a new instance of the Notification class.
        /// </summary>
        /// <param name="name">Gets a unique name identifying the notification.</param>
        /// <param name="notificationType">Identifies the type of notification.</param>
        internal Notification(string name, NotificationType notificationType)
        {
            Throw.IfArgumentNullOrWhitespace(name, nameof(name));

            this.Id = Guid.NewGuid();
            this.Name = name;
            this.NotificationType = notificationType;

            this.Attributes = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets the id for the notification.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name used to identify the notification.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value identifying the type of notification for the current instance.
        /// </summary>
        public NotificationType NotificationType { get; private set; }

        /// <summary>
        /// Gets a list of values for the notification to associate additional information that won't be sent to the client.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Attributes { get; private set; }

        /// <summary>
        /// Adds an attribute to the current notification.
        /// </summary>
        /// <param name="key">A key for the attribute's value.</param>
        /// <param name="value">The attribute's value.</param>
        public void AddAttribute(string key, string value)
        {
            Throw.IfArgumentNullOrWhitespace(key, nameof(key));
            Throw.IfArgumentNullOrWhitespace(value, nameof(value));

            this.Attributes = this.Attributes.Concat(Enumerable.Repeat(new KeyValuePair<string, string>(key, value), 1)).ToArray();
        }

        /// <summary>
        /// Attempts ot get an attribute based on the provided key and retruns null if an attribute does not exist.
        /// </summary>
        /// <param name="key">The key for the attribute.</param>
        /// <returns>The attribute value or null.</returns>
        public string TryGetAttribute(string key)
        {
            KeyValuePair<string, string> keyValuePair = this.Attributes.SingleOrDefault(item => item.Key == key);

            if (keyValuePair.Equals(default(KeyValuePair<string, string>)))
            {
                return null;
            }

            return keyValuePair.Value;
        }

        /// <summary>
        /// Gets an attribute based on the provided key.
        /// </summary>
        /// <param name="key">The key for the attribute.</param>
        /// <returns>The attribute value.</returns>
        public string GetAttribute(string key)
        {
            string value = this.TryGetAttribute(key);

            if (value == null)
            {
                throw new KeyNotFoundException("Content for key (" + key + ") not found.");
            }

            return value;
        }
    }
}