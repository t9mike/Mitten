namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// Represents the result of sending push notification messages.
    /// </summary>
    public class PushNotificationResult
    {
        private PushNotificationResult(bool wasSuccessful, string endpointName, string errorMessage = null)
        {
            Throw.IfArgumentNullOrWhitespace(endpointName, nameof(endpointName));

            this.WasSuccessful = wasSuccessful;
            this.EndpointName = endpointName;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets whether or not a notification was sent successfully.
        /// </summary>
        public bool WasSuccessful { get; private set; }

        /// <summary>
        /// Gets the name of the end-point where the notification was sent to.
        /// </summary>
        public string EndpointName { get; private set; }

        /// <summary>
        /// Gets an error message if one occurred.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="endpointName">The name of the end-point where the notification was sent to.</param>
        /// <returns>A new instance of the PushNotificationResult class.</returns>
        public static PushNotificationResult Success(string endpointName)
        {
            return new PushNotificationResult(true, endpointName);
        }
            
        /// <summary>
        /// Creates a new failure result.
        /// </summary>
        /// <param name="endpointName">The name of the end-point where the notification was sent to.</param>
        /// <param name="errorMessage">A message containing the details of the failure.</param>
        /// <returns>A new instance of the PushNotificationResult class.</returns>
        public static PushNotificationResult Failed(string endpointName, string errorMessage)
        {
            Throw.IfArgumentNullOrWhitespace(errorMessage, nameof(errorMessage));
            return new PushNotificationResult(false, endpointName, errorMessage);
        }
    }
}
