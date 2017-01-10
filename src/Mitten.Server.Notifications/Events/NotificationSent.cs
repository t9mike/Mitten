namespace Mitten.Server.Notifications.Events
{
    /// <summary>
    /// Occurs when a notification has been sent.
    /// </summary>
    public class NotificationSent : NotificationEvent
    {
        /// <summary>
        /// Initializes a new instance of the NotificationSent class.
        /// </summary>
        /// <param name="notification">The inotification that has been sent.</param>
        /// <param name="accountId">The id of the account that the notification was sent to.</param>
        /// <param name="destination">The id of the destination for the notification.</param>
        /// <param name="endpointName">The endpoint the notification was intended for.</param>
        internal NotificationSent(Notification notification, string accountId, string destination, string endpointName)
            : base(notification)
        {
            Throw.IfArgumentNullOrWhitespace(accountId, nameof(accountId));

            this.AccountId = accountId;
            this.Destination = destination;
            this.EndpointName = endpointName;
        }

        /// <summary>
        /// Gets the id of the account the notification was sent to.
        /// </summary>
        public string AccountId { get; private set; }

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