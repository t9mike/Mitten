using Mitten.Mobile.Themes;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Provides access to a device's status bar. The status bar is typically found at 
    /// the top and contains device status information such as battery life and cellular service.
    /// </summary>
    public interface IDeviceStatusBar
    {
        /// <summary>
        /// Sets the visual style for the status bar.
        /// </summary>
        /// <param name="statusBarStyle">The status bar style.</param>
        void SetStatusBarStyle(StatusBarStyle statusBarStyle);
    }
}
