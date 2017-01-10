namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Provides access to a device's settings.
    /// </summary>
    public interface IDeviceSettings
    {
        /// <summary>
        /// Determines whether or not the current device supports navigating to the external privacy settings for the app.
        /// </summary>
        /// <returns>True if the device supports navigating to the external privacy settings, otherwise false.</returns>
        bool CanNavigateToPrivacySettings();

        /// <summary>
        /// Navigates the user to the external privacy settings for the device.
        /// </summary>
        void NavigateToPrivacySettings();
    }
}
