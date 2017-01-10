using System;
using System.Threading.Tasks;

namespace Mitten.Mobile.Devices
{
    /// <summary>
    /// Defines an interface for a device's camera.
    /// </summary>
    public interface ICameraDevice
    {
        /// <summary>
        /// Gets the current authorization permissions for the camera.
        /// </summary>
        CameraPermission GetPermission();

        /// <summary>
        /// Gets whether or not the camera for the current device is on and ready to capture an image.
        /// </summary>
        /// <returns>True if the camera is on, otherwise false.</returns>
        bool IsCameraOn();

        /// <summary>
        /// Gets whether or not the camera has a flash.
        /// </summary>
        /// <returns>True if the camera flash is supported, otherwise false.</returns>
        bool IsFlashAvailable();

        /// <summary>
        /// Gets the current mode of the camera's flash.
        /// </summary>
        /// <returns>The current flash mode.</returns>
        CameraFlashMode GetCurrentFlashMode();

        /// <summary>
        /// Sets the camera's flash mode.
        /// </summary>
        /// <param name="flashMode">The mode to set.</param>
        void SetFlashMode(CameraFlashMode flashMode);

        /// <summary>
        /// Requests user access to the device.
        /// </summary>
        /// <returns>True if access was granted, otherwise false.</returns>
        Task<bool> RequestAccessAsync();

        /// <summary>
        /// Begins starting the camera.
        /// </summary>
        /// <param name="cameraStarted">Invoked when the camera has been started.</param>
        void BeginStartCamera(Action<object> cameraStarted);

        /// <summary>
        /// Stops the camera.
        /// </summary>
        void StopCamera();

        /// <summary>
        /// Begins capturing an image from the camera.
        /// </summary>
        /// <param name="imageCaptured">Invoked when an image has been captured.</param>
        /// <param name="captureFailed">Invoked when a failure occured while trying to capture an image.</param>
        void BeginCaptureImage(Action<byte[]> imageCaptured, Action<string> captureFailed);
    }
}