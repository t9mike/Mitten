using Mitten.Mobile.Application;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Application
{
    /// <summary>
    /// Provides access to the status bar on an iOS device.
    /// </summary>
    public class iOSDeviceStatusBar : IDeviceStatusBar
    {
        /// <summary>
        /// Sets the visual style for the status bar.
        /// </summary>
        /// <param name="statusBarStyle">The status bar style.</param>
        public void SetStatusBarStyle(StatusBarStyle statusBarStyle)
        {
            UIStatusBarStyle status = UIStatusBarStyle.Default;

            if (statusBarStyle == StatusBarStyle.Light)
            {
                status = UIStatusBarStyle.LightContent;
            }

            UIApplication.SharedApplication.SetStatusBarStyle(status, true);
        }
    }
}
