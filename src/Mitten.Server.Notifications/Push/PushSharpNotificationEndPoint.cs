using System;
using System.Threading.Tasks;
using PushSharp.Core;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// Base class for objects using PushSharp to represent an end-point for sending notification 
    /// messages to a platform specific notification service.
    /// </summary>
    public abstract class PushSharpNotificationEndPoint<TPushSharpNotification> : IPushSharpNotificationEndPoint
         where TPushSharpNotification : INotification
    {
        private IServiceConnection<TPushSharpNotification> serviceConnection;

        /// <summary>
        /// Initializes a new instance of the PushSharpNotificationEndPoint class.
        /// </summary>
        /// <param name="endPointName">A name for the end-point.</param>
        internal PushSharpNotificationEndPoint(string endPointName)
        {
            Throw.IfArgumentNullOrWhitespace(endPointName, "endPointName");
            this.EndPointName = endPointName;
        }

        /// <summary>
        /// Gets the name of the end-point.
        /// </summary>
        public string EndPointName { get; private set; }

        /// <summary>
        /// Sends the specified push notification.
        /// </summary>
        /// <param name="mobileDevice">The mobile device to send the notification to.</param>
        /// <param name="notification">The notification to send.</param>
        /// <returns>The task and result of the operation.</returns>
        public abstract Task<PushNotificationResult> SendNotification(MobileDevice mobileDevice, PushNotification notification);

        /// <summary>
        /// Sends a PushSharp notification to the service end-point.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns>The task and result of the operation.</returns>
        protected async Task<PushNotificationResult> SendPushSharpNotification(TPushSharpNotification notification)
        {
            if (this.serviceConnection == null)
            {
                throw new InvalidOperationException("The end-point has not been connected.");
            }

            try
            {
                // TODO: it appears we can optimize this by creating our own connection interface that does not throw the exception but instead returns it
                // the underlying APNS service connection uses an APNS connection object that returns an exception but the service connection object throws it
                await this.serviceConnection.Send(notification).ConfigureAwait(false);
                return PushNotificationResult.Success(this.EndPointName);
            }
            catch (Exception ex)
            {
                PushNotificationResult result = this.HandleFailedNotificationResult(notification, ex);

                if (result != null)
                {
                    return result;
                }

                throw;
            }
        }

        /// <summary>
        /// Handles a failed notification and returns the result for the failure or null if the exception should be rethrown. 
        /// </summary>
        /// <param name="notification">The notification that failed to send.</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>A notification result or null.</returns>
        protected abstract PushNotificationResult HandleFailedNotificationResult(TPushSharpNotification notification, Exception ex);

        /// <summary>
        /// Sets the service connection for the end-point.
        /// </summary>
        /// <param name="serviceConnection">A PushSharp service connection.</param>
        protected void SetConnection(IServiceConnection<TPushSharpNotification> serviceConnection)
        {
            Throw.IfArgumentNull(serviceConnection, nameof(serviceConnection));

            if (this.serviceConnection != null)
            {
                throw new InvalidOperationException("The end-point has already been connected.");
            }

            this.serviceConnection = serviceConnection;
        }
    }
}
