using System;
using Mitten.Mobile.Application.Components;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Provides access to the media library on an iOS device.
    /// </summary>
    public class iOSMediaLibrary : ImageComponent, IMediaLibrary
    {
        /// <summary>
        /// Initializes a new instance of the iOSMediaLibrary class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting any image controls.</param>
        public iOSMediaLibrary(Func<UIViewController> getPresentingController)
            : base(getPresentingController)
        {
        }

        /// <summary>
        /// Determines whether or not the user is allowed to browse photos on the device.
        /// </summary>
        /// <returns>True if photos is available, otherwise false.</returns>
        public bool CanBrowsePhotos()
        {
            return UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.PhotoLibrary);
        }

        /// <summary>
        /// Browses the photos on the current device and allow the user the ability to choose an image.
        /// </summary>
        /// <param name="imagePicked">Gets invoked when the user has picked an image.</param>
        /// <param name="browseCanceled">Gets invoked when the user has canceled browsing.</param>
        public void BrowsePhotos(Action<byte[]> imagePicked, Action browseCanceled)
        {
            this.PresentImagePicker(UIImagePickerControllerSourceType.PhotoLibrary, null, imagePicked, browseCanceled);
        }
    }
}
