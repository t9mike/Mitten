using System;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Represents the result of parsing the received data from a push notification.
    /// </summary>
    public class PushNotificationParseResult
    {
        /// <summary>
        /// Defines codes representing the expected results.
        /// </summary>
        public enum ResultCode
        {
            /// <summary>
            /// Invalid.
            /// </summary>
            Invalid = 0,

            /// <summary>
            /// Indicates that a notification was successfully parsed.
            /// </summary>
            Success,

            /// <summary>
            /// Indicates that the name of the received notification is unknown.
            /// </summary>
            UnknownName,

            /// <summary>
            /// Indicates that received content for a notification is missing the notification's name.
            /// </summary>
            MissingName,

            /// <summary>
            /// Indicates that a notification failed to initialize from the data received.
            /// </summary>
            FailedToInitialize,
        }

        /// <summary>
        /// Initializes a new instance of the PushNotificationParseResult class.
        /// </summary>
        /// <param name="resultCode">A code representing the result of the operation.</param>
        internal PushNotificationParseResult(ResultCode resultCode)
            : this(resultCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PushNotificationParseResult class.
        /// </summary>
        /// <param name="notification">The recieved notification if successful.</param>
        internal PushNotificationParseResult(PushNotification notification)
            : this(ResultCode.Success, notification)
        {
        }

        private PushNotificationParseResult(ResultCode resultCode, PushNotification notification)
        {
            if (resultCode == ResultCode.Invalid)
            {
                throw new ArgumentException("Invalid result code.", nameof(resultCode));
            }

            if (resultCode == ResultCode.Success && notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "The notification argument must not be null if the result code is success.");
            }

            this.Result = resultCode;
            this.Notification = notification;
        }

        /// <summary>
        /// Gets the notification if it was successfully parsed.
        /// </summary>
        public PushNotification Notification { get; private set; }

        /// <summary>
        /// Gets the code that represents one of the expected results.
        /// </summary>
        public ResultCode Result { get; private set; }
    }
}