using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;

namespace Mitten.Server.Notifications.Push
{
    /// <summary>
    /// An end-point for sending notifications to iOS devices.
    /// </summary>
    /// <remarks>
    /// 
    /// It is important to use the correct certificate. If the app is provisioned using a development
    /// provisioning profile, then a development certificate should be used. However, if the app
    /// is provisioned for AppStore/Ad-Hoc, the production certificate should be used.
    /// 
    /// </remarks>
    public class iOSNotificationEndPoint : PushSharpNotificationEndPoint<ApnsNotification>
    {
        private static class Constants
        {
            public const string JsonPayload = @"
{{
    ""aps"": {{ ""alert"" : ""{0}"" }},
    ""name"": ""{1}""
}}";
        }
        
        private iOSNotificationEndPoint()
            : base("Apple Push Notification Service")
        {
        }

        /// <summary>
        /// Sends the specified push notification.
        /// </summary>
        /// <param name="mobileDevice">The mobile device to send the notification to.</param>
        /// <param name="notification">The notification to send.</param>
        /// <returns>The task and result of the operation.</returns>
        public override Task<PushNotificationResult> SendNotification(MobileDevice mobileDevice, PushNotification notification)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            sb.AppendFormat(@"""aps"": {{ ""alert"" : ""{0}"" }},", notification.AlertText);
            sb.AppendFormat(@"""name"": ""{0}""", notification.Name);

            int count = notification.Content.Count();
            for (int i = 0; i < count; i++)
            {
                KeyValuePair<string, string> value = notification.Content.ElementAt(i);

                sb.Append(",");
                sb.AppendFormat(@"""{0}"": ""{1}""", value.Key, value.Value);
            }

            sb.Append("}");

            JObject payload = JObject.Parse(sb.ToString());
            ApnsNotification apnsNotification = new ApnsNotification(mobileDevice.PushNotificationToken, payload);
            
            return this.SendPushSharpNotification(apnsNotification);
        }

        /// <summary>
        /// Handles a failed notification and returns the result for the failure or null if the exception should be rethrown. 
        /// </summary>
        /// <param name="notification">The notification that failed to send.</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>A notification result or null.</returns>
        protected override PushNotificationResult HandleFailedNotificationResult(ApnsNotification notification, Exception ex)
        {
            ApnsNotificationException notificationException = ex as ApnsNotificationException;
            if (notificationException != null)
            {
                if (notificationException.ErrorStatusCode == ApnsNotificationErrorStatusCode.ConnectionError)
                {
                    return
                        PushNotificationResult.Failed(
                            this.EndPointName,
                            "Connection error: " + ex.Message);
                }

                return
                    PushNotificationResult.Failed(
                        this.EndPointName,
                        "Failed to send notification (" + notificationException.Notification.Identifier + ") with status (" + notificationException.ErrorStatusCode + ") and error message: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Creates a new iOS end-point to connect to the APNS production services.
        /// </summary>
        /// <param name="certificate">The data for the certificate.</param>
        /// <param name="certificatePassword">The password for the certificate.</param>
        /// <returns>A new iOS end-point.</returns>
        public static iOSNotificationEndPoint CreateForProduction(byte[] certificate, string certificatePassword)
        {
            return iOSNotificationEndPoint.Create(certificate, certificatePassword, ApnsConfiguration.ApnsServerEnvironment.Production);
        }

        /// <summary>
        /// Creates a new iOS end-point to connect to the APNS sandbox services.
        /// </summary>
        /// <param name="certificate">The data for the certificate.</param>
        /// <param name="certificatePassword">The password for the certificate.</param>
        /// <returns>A new iOS end-point.</returns>
        public static iOSNotificationEndPoint CreateForSandbox(byte[] certificate, string certificatePassword)
        {
            return iOSNotificationEndPoint.Create(certificate, certificatePassword, ApnsConfiguration.ApnsServerEnvironment.Sandbox);
        }

        private static iOSNotificationEndPoint Create(byte[] certificate, string certificatePassword, ApnsConfiguration.ApnsServerEnvironment environment)
        {
            Throw.IfArgumentNull(certificate, nameof(certificate));
            Throw.IfArgumentNullOrWhitespace(certificatePassword, nameof(certificatePassword));
             
            iOSNotificationEndPoint endPoint = new iOSNotificationEndPoint();
            ApnsConfiguration configuration = new ApnsConfiguration(environment, certificate, certificatePassword);

            endPoint.SetConnection(new ApnsServiceConnection(configuration));

            return endPoint;
        }
    }
}