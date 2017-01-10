using System;

namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Defines an component that provides access to a device's media library.
    /// </summary>
    public interface IMediaLibrary
    {
        /// <summary>
        /// Determines whether or not the user is allowed to browse photos on the device.
        /// </summary>
        /// <returns>True if photos is available, otherwise false.</returns>
        bool CanBrowsePhotos();

        /// <summary>
        /// Browses the photos on the current device and allow the user the ability to choose an image.
        /// </summary>
        /// <param name="imagePicked">Gets invoked when the user has picked an image.</param>
        /// <param name="browseCanceled">Gets invoked when the user has canceled browsing.</param>
        void BrowsePhotos(Action<byte[]> imagePicked, Action browseCanceled);
    }
}
