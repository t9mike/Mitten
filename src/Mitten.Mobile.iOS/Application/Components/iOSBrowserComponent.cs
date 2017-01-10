using Foundation;
using Mitten.Mobile.Application.Components;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Provides access to the web browser on an iOS device.
    /// </summary>
    public class iOSBrowserComponent : IBrowserComponent
    {
        /// <summary>
        /// Determines whether or not the current device supports browsing websites through an external browser.
        /// </summary>
        /// <param name="url">The url to browse to.</param>
        /// <returns>True if the device supports browsing websites through an external browser.</returns>
        public bool CanBrowseWebsite(string url)
        {
            return UIApplication.SharedApplication.CanOpenUrl(new NSUrl(url));
        }

        /// <summary>
        /// Browses to a website with the specified url.
        /// </summary>
        /// <param name="url">The url to browse to.</param>
        public void BrowseWebsite(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }
    }
}
