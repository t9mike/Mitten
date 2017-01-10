using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Mitten.Mobile.iOS.Application.PushNotifications
{
    /// <summary>
    /// Utility class to handle converting recieved remote notification data into PushNotification objects.
    /// </summary>
    internal static class iOSPushNotificationConverter
    {
        private static class Constants
        {
            public const string LuanchPushNotificationKey = "UIApplicationLaunchOptionsRemoteNotificationKey";
        }

        /// <summary>
        /// Creates a dictionary representing the values from a push notification from the provided launch options, or null if no notifications were present.
        /// </summary>
        /// <param name="launchOptions">The launch options.</param>
        /// <returns>A PushNotification or null.</returns>
        public static IDictionary<string, string> FromLaunchOptions(NSDictionary launchOptions)
        {
            NSString key = new NSString(Constants.LuanchPushNotificationKey);
            if (launchOptions != null && launchOptions.ContainsKey(key))
            {
                NSObject notificationData;
                if (launchOptions.TryGetValue(key, out notificationData))
                {
                    return iOSPushNotificationConverter.ConvertDictionary((NSDictionary)notificationData);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a push notification from the provided information received from a remote notification, or null
        /// if the data provided was invalid.
        /// </summary>
        /// <param name="userInfo">The data received.</param>
        /// <returns>A PushNotification or null.</returns>
        public static IDictionary<string, string> FromRemoteNotification(NSDictionary userInfo)
        {
            return iOSPushNotificationConverter.ConvertDictionary(userInfo);
        }

        private static Dictionary<string, string> ConvertDictionary(NSDictionary dictionary)
        {
            return
                dictionary.ToDictionary(
                    item => item.Key.ToString(),
                    item => item.Value.ToString());
        }
    }
}