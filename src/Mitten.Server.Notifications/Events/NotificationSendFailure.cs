namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Occurs when a notification failed while sending.
    /// </summary>
    public class NotificationSendFailure : NotificationEvent
    {
        /// <summary>
        /// Initializes a new instance of the NotificationSendFailure class.
        /// </summary>
        /// <param name="notification">The notification that failed to send.</param>
        /// <param name="errorCode">The error code for the notification.</param>
        /// <param name="accountId">The id of the account the notification was intended for.</param>
        /// <param name="description">A description of the failure.</param>
        /// <param name="destination">The id of the destination for the notification.</param>
        /// <param name="endpointName">The endpoint the notification was intended for.</param>
        internal NotificationSendFailure(
            Notification notification, 
            NotificationErrorCode errorCode, 
            string accountId,
            string description,
            string destination = null, 
            string endpointName = null)
            : base(notification)
        {
            Throw.IfArgumentNullOrWhitespace(description, nameof(description));

            this.AccountId = accountId;
            this.ErrorCode = errorCode;
            this.Description = description;
            this.Destination = destination;
            this.EndpointName = endpointName;
        }

        /// <summary>
        /// Gets the id of the account the notification was intended for.
        /// </summary>
        public string AccountId { get; private set; }

        /// <summary>
        /// Gets the error code for the notification.
        /// </summary>
        public NotificationErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Gets a description of the failure.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value identifying the destination for the notification, such as a mobile device id or email address.
        /// </summary>
        public string Destination { get; private set; }

        /// <summary>
        /// Gets the name of the end point the notification was intended for, such as 
        /// the name of the server responsible for sending the notification to the client.
        /// </summary>
        public string EndpointName { get; private set; }
    }
}
