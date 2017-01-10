using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using Mitten.Mobile.Devices;

namespace Mitten.Mobile.iOS.Devices
{
    /// <summary>
    /// Represents the camera for an iOS device.
    /// </summary>
    /// <remarks>
    /// For the camera device we will be using the AV Mitten framework.
    /// https://developer.apple.com/library/ios/documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html
    /// </remarks>
    public class CameraDevice : ICameraDevice
    {
        private readonly AVCaptureDevice device;
        private AVCaptureSession session;
        private AVCaptureStillImageOutput output;

        /// <summary>
        /// Initializes a new instance of the CameraDevice class.
        /// </summary>
        internal CameraDevice()
        {
            this.device = AVCaptureDevice.Devices.SingleOrDefault(device => device.Position == AVCaptureDevicePosition.Back);
        }

        /// <summary>
        /// Gets the current authorization permissions for the camera.
        /// </summary>
        public CameraPermission GetPermission()
        {
            if (this.device == null)
            {
                return CameraPermission.NotAvailable;
            }

            AVAuthorizationStatus status = this.GetAuthorizationStatus();

            if (status == AVAuthorizationStatus.Authorized)
            {
                return CameraPermission.Authorized;
            }

            if (status == AVAuthorizationStatus.Denied)
            {
                return CameraPermission.Denied;
            }

            if (status == AVAuthorizationStatus.NotDetermined)
            {
                return CameraPermission.NotDetermined;
            }

            if (status == AVAuthorizationStatus.Restricted)
            {
                return CameraPermission.Restricted;
            }

            throw new CameraPermissionException("Unexpected AVAuthorizationStatus (" + status + ").");
        }

        /// <summary>
        /// Gets whether or not the camera for the current device is on and ready to capture an image.
        /// </summary>
        public bool IsCameraOn()
        {
            return this.session != null && this.session.Running;
        }

        /// <summary>
        /// Gets whether or not the camera has a flash.
        /// </summary>
        /// <returns>True if the camera flash is supported, otherwise false.</returns>
        public bool IsFlashAvailable()
        {
            return 
                this.device != null &&
                this.device.FlashAvailable;
        }

        /// <summary>
        /// Gets the current mode of the camera's flash.
        /// </summary>
        /// <returns>The current flash mode.</returns>
        public CameraFlashMode GetCurrentFlashMode()
        {
            if (this.device == null || !this.IsFlashAvailable())
            {
                return CameraFlashMode.NotAvailable;
            }

            if (this.device.FlashMode == AVCaptureFlashMode.On)
            {
                return CameraFlashMode.On;
            }

            if (this.device.FlashMode == AVCaptureFlashMode.Auto)
            {
                return CameraFlashMode.Auto;
            }

            if (this.device.FlashMode == AVCaptureFlashMode.Off)
            {
                return CameraFlashMode.Off;
            }

            throw new InvalidOperationException("Unexpected AVCaptureDevice.FlashMode (" + this.device.FlashMode + ").");
        }

        /// <summary>
        /// Sets the camera's flash mode.
        /// </summary>
        /// <param name="flashMode">The mode to set.</param>
        public void SetFlashMode(CameraFlashMode flashMode)
        {
            if (!this.IsFlashAvailable())
            {
                throw new InvalidOperationException("The camera's flash is not available.");
            }

            NSError error;
            if (!this.device.LockForConfiguration(out error))
            {
                throw new NSErrorException(error);
            }

            try
            {
                if (flashMode == CameraFlashMode.On)
                {
                    this.device.FlashMode = AVCaptureFlashMode.On;
                }
                else if (flashMode == CameraFlashMode.Auto)
                {
                    this.device.FlashMode = AVCaptureFlashMode.Auto;
                }
                else if (flashMode == CameraFlashMode.Off)
                {
                    this.device.FlashMode = AVCaptureFlashMode.Off;
                }
                else
                {
                    throw new ArgumentException("Invalid CameraFlashMode (" + flashMode + ").");
                }
            }
            finally
            {
                this.device.UnlockForConfiguration();
            }
        }

        /// <summary>
        /// Requests user access to the device.
        /// </summary>
        /// <returns>True if access was granted, otherwise false.</returns>
        public Task<bool> RequestAccessAsync()
        {
            return AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
        }

        /// <summary>
        /// Begins starting the camera.
        /// </summary>
        /// <param name="cameraStarted">Invoked when the camera has been started. An instance of the current session will be passed as the method's argument.</param>
        public void BeginStartCamera(Action<object> cameraStarted)
        {
            if (this.GetPermission() != CameraPermission.Authorized)
            {
                throw new InvalidOperationException("Camera access not authorized (" + this.GetPermission() + ").");
            }

            if (this.IsCameraOn())
            {
                throw new InvalidOperationException("The camera is already on.");
            }

            this.InitializeSession();
            this.session.StartRunning();
            cameraStarted(this.session);
        }

        /// <summary>
        /// Stops the camera.
        /// </summary>
        public void StopCamera()
        {
            if (!this.IsCameraOn())
            {
                throw new InvalidOperationException("The camera has not been started.");
            }

            this.session.StopRunning();
            this.CleanUp();
        }

        /// <summary>
        /// Begins capturing an image an image from the camera.
        /// </summary>
        /// <param name="imageCaptured">Invoked when an image has been captured.</param>
        /// <param name="captureFailed">Invoked when a failure occured while trying to capture an image.</param>
        public void BeginCaptureImage(Action<byte[]> imageCaptured, Action<string> captureFailed)
        {
            if (!this.IsCameraOn())
            {
                throw new InvalidOperationException("The camera has not been started.");
            }

            AVCaptureConnection connection = this.output.ConnectionFromMediaType(AVMediaType.Video);
            this.output.CaptureStillImageAsynchronously(
                connection, 
                (buffer, error) => this.OnImageCaptured(buffer, error, imageCaptured, captureFailed));
        }
            
        private void OnImageCaptured(
            CMSampleBuffer buffer, 
            NSError error, 
            Action<byte[]> imageCaptured,
            Action<string> captureFailed)
        {
            if (error != null)
            {
                captureFailed(error.LocalizedDescription);
            }
            else
            {
                NSData data = AVCaptureStillImageOutput.JpegStillToNSData(buffer);

                byte[] image = new byte[data.Length];
                Marshal.Copy(
                    data.Bytes, 
                    image, 
                    0, 
                    Convert.ToInt32(data.Length));

                imageCaptured(image);
            }
        }

        private void CleanUp()
        {
            this.session.Dispose();
            this.session = null;

            this.output.Dispose();
            this.output = null;
        }

        private void InitializeSession()
        {
            if (this.session != null)
            {
                throw new InvalidOperationException("A session is currently active.");
            }

            this.session = new AVCaptureSession();

            this.session.BeginConfiguration();
            this.session.SessionPreset = AVCaptureSession.PresetPhoto;

            NSError error;
            AVCaptureDeviceInput deviceInput = AVCaptureDeviceInput.FromDevice(this.device, out error);

            if (deviceInput == null)
            {
                throw new NSErrorException(error);
            }

            this.session.AddInput(deviceInput);

            this.InitializeOutput();
            this.session.AddOutput(this.output);

            session.CommitConfiguration();
        }

        private void InitializeOutput()
        {
            if (this.output != null)
            {
                throw new InvalidOperationException("An image output is currently active.");
            }

            this.output = new AVCaptureStillImageOutput();
            this.output.OutputSettings = new NSDictionary(AVVideo.CodecKey, AVVideo.CodecJPEG);
        }

        private AVAuthorizationStatus GetAuthorizationStatus()
        {
            return AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
        }
    }
}