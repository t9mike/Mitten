namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Defines an component that provides access to a device's web browser.
    /// </summary>
    public interface IBrowserComponent
    {
        /// <summary>
        /// Determines whether or not the current device supports browsing websites through an external browser.
        /// </summary>
        /// <param name="url">The url to browse to.</param>
        /// <returns>True if the device supports browsing websites through an external browser.</returns>
        bool CanBrowseWebsite(string url);

        /// <summary>
        /// Browses to a website with the specified url.
        /// </summary>
        /// <param name="url">The url to browse to.</param>
        void BrowseWebsite(string url);
    }
}
