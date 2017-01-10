using System.Threading.Tasks;
using Mitten.Mobile.Identity;
using Mitten.Mobile.Remote;
using Mitten.Mobile.System;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Handles registering a device and user account with a remote server to receive push notifications.
    /// </summary>
    public interface IPushNotificationServerRegistration
    {
        /// <summary>
        /// Registers a push notification token for the current device assigned to the specified account.
        /// </summary>
        /// <param name="account">An account.</param>
        /// <param name="platformType">The type of platform running on the device.</param>
        /// <param name="appVersion">The current version of the application.</param>
        /// <param name="notificationToken">A unique token identifying the device when sending notifications.</param>
        /// <returns>The result of the service request.</returns>
        Task<ServiceResult> RegisterNotificationToken(IAccount account, PlatformType platformType, string appVersion, string notificationToken);
    }
}
