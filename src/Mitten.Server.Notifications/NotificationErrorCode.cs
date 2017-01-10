namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Represents the different types of errors that could be encountered while sending a notification.
    /// </summary>
    public enum NotificationErrorCode
    {
        /// <summary>
        /// Indicates a notification could not be sent because an associated notification account could not be found.
        /// </summary>
        AccountNotFound,

        /// <summary>
        /// Indicates that the notification couldn't be sent because an application or user has explicitly disabled or denied the notification. 
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that a notification could not be sent because a mobile device for an account has not been registered.
        /// </summary>
        MobileDeviceNotRegistered,

        /// <summary>
        /// Indicates that a notification was not sent due to a version mismatch between the server and a receiving client.
        /// </summary>
        VersionMismatch,

        /// <summary>
        /// Indicates the notification failed communicating with a third-party endpoint
        /// responsible for routing notifications to its final destination.
        /// </summary>
        RouteFailure,
    }
}