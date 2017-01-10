using System;
using Mitten.Mobile.Application.Components;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Provides high-level access to the camera on an iOS device.
    /// </summary>
    public class iOSCameraComponent : ImageComponent, ICameraComponent
    {
        /// <summary>
        /// Initializes a new instance of the iOSCameraComponent class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting any image controls.</param>
        public iOSCameraComponent(Func<UIViewController> getPresentingController)
            : base(getPresentingController)
        {
        }

        /// <summary>
        /// Determines whether or not the camera is available.
        /// </summary>
        /// <returns>True if a camera is available and a photo can be taken, otherwise false.</returns>
        public bool CanTakePhoto()
        {
            return UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
        }

        /// <summary>
        /// Takes a photo using the device's camera.
        /// </summary>
        /// <param name="photoTaken">Gets invoked when the user has taken a photo.</param>
        /// <param name="cameraPosition">Identifies the postion of the camera on the device to use.</param>
        /// <param name="actionCanceled">Gets invoked when the user canceled the operation.</param>
        public void TakePhoto(Action<byte[]> photoTaken, CameraPosition cameraPosition, Action actionCanceled)
        {
            this.PresentImagePicker(UIImagePickerControllerSourceType.Camera, cameraPosition, photoTaken, actionCanceled);
        }
    }
}
