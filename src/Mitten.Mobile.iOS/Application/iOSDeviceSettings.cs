using Foundation;
using Mitten.Mobile.Application;
using UIKit;

namespace Mitten.Mobile.iOS.Application
{
    /// <summary>
    /// Provides access to the settings on an iOS device.
    /// </summary>
    public class iOSDeviceSettings : IDeviceSettings
    {
        /// <summary>
        /// Determines whether or not the current device supports navigating to the external privacy settings for the app.
        /// </summary>
        /// <returns>True if the device supports navigating to the external privacy settings, otherwise false.</returns>
        public bool CanNavigateToPrivacySettings()
        {
            return !string.IsNullOrWhiteSpace(UIApplication.OpenSettingsUrlString);
        }

        /// <summary>
        /// Navigates the user to the external privacy settings for the device.
        /// </summary>
        public void NavigateToPrivacySettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }
    }
}
