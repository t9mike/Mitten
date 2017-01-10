using System;

namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Represents an error that occurred internally within the notification component.
    /// </summary>
    public class NotificationInternalError : NotificationEvent
    {
        /// <summary>
        /// Initializes a new instance of the NotificationInternalError class.
        /// </summary>
        /// <param name="notification">The notification that caused the error.</param>
        /// <param name="accountId">An account id related to the error.</param>
        /// <param name="message">A message describing the error.</param>
        /// <param name="exception">The exception which contains the error details.</param>
        internal NotificationInternalError(Notification notification, string accountId, string message, Exception exception)
            : base(notification)
        {
            this.AccountId = accountId;
            this.Message = message;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the id of the account the notification was intended for.
        /// </summary>
        public string AccountId { get; }

        /// <summary>
        /// Gets the message describing the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the exception which contains the error details.
        /// </summary>
        public Exception Exception { get; }
    }
}