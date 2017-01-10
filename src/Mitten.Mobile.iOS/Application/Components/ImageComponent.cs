using System;
using Mitten.Mobile.Application.Components;
using Mitten.Mobile.iOS.Graphics;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Base class for components that deal with images/photos.
    /// </summary>
    public abstract class ImageComponent
    {
        private readonly Func<UIViewController> getPresentingController;

        /// <summary>
        /// Initializes a new instance of the ImageComponent class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting any image controls.</param>
        internal ImageComponent(Func<UIViewController> getPresentingController)
        {
            Throw.IfArgumentNull(getPresentingController, nameof(getPresentingController));
            this.getPresentingController = getPresentingController;
        }

        /// <summary>
        /// Presents an image picker.
        /// </summary>
        /// <param name="sourceType">The picker source.</param>
        /// <param name="cameraPosition">The position for the camera if the camera is used.</param>
        /// <param name="actionComplete">An action that will get invoked when complete.</param>
        /// <param name="actionCanceled">An action that will get invoked when canceled.</param>
        protected void PresentImagePicker(UIImagePickerControllerSourceType sourceType, CameraPosition? cameraPosition, Action<byte[]> actionComplete, Action actionCanceled)
        {
            UIViewController presentingController = this.getPresentingController();
            UIImagePickerController imagePickerController = new UIImagePickerController();

            imagePickerController.AllowsEditing = true;
            imagePickerController.SourceType = sourceType;

            if (cameraPosition == CameraPosition.Front)
            {
                imagePickerController.CameraDevice = UIImagePickerControllerCameraDevice.Front;
            }

            imagePickerController.FinishedPickingMedia += (sender, e) => this.HandleFinishedFinishedPickingMedia(imagePickerController, e, actionComplete);
            imagePickerController.Canceled += (sender, e) => this.HandleCanceled(imagePickerController, actionCanceled);

            presentingController.PresentViewController(imagePickerController, true, () => { });
        }

        private void HandleFinishedFinishedPickingMedia(
            UIImagePickerController imagePickerController,
            UIImagePickerMediaPickedEventArgs e,
            Action<byte[]> actionComplete)
        {
            UIImage image = e.Info[UIImagePickerController.EditedImage] as UIImage;

            if (image == null)
            {
                image = e.Info[UIImagePickerController.OriginalImage] as UIImage;
            }

            if (image == null)
            {
                throw new InvalidOperationException("An image was not found from the selection.");
            }

            imagePickerController.DismissViewController(true, () => { });
            actionComplete(UIImageByteConverter.ToBytesAsJpeg(image));
        }

        private void HandleCanceled(UIImagePickerController imagePickerController, Action actionCanceled)
        {
            imagePickerController.DismissViewController(true, () => { });
            actionCanceled();
        }
    }
}
