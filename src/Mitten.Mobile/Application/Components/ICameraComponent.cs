using System;

namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Defines an component that provides access to a device's camera.
    /// </summary>
    /// <remarks>
    /// This provides high-level access for taking pictures and is intended
    /// to take advantage of the device's existing user interface. For lower-level
    /// access to the camera, see the camera classes found in the Devices namespace.
    /// </remarks>
    public interface ICameraComponent
    {
        /// <summary>
        /// Determines whether or not the camera is available.
        /// </summary>
        /// <returns>True if a camera is available and a photo can be taken, otherwise false.</returns>
        bool CanTakePhoto();

        /// <summary>
        /// Takes a photo using the device's camera.
        /// </summary>
        /// <param name="photoTaken">Gets invoked when the user has taken a photo.</param>
        /// <param name="cameraPosition">Identifies the postion of the camera on the device to use.</param>
        /// <param name="actionCanceled">Gets invoked when the user canceled the operation.</param>
        void TakePhoto(Action<byte[]> photoTaken, CameraPosition cameraPosition, Action actionCanceled);
    }
}
