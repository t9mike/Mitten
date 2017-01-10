using System;

namespace Mitten.Mobile.iOS.Devices
{
    /// <summary>
    /// Represents an error related to camera permissions.
    /// </summary>
    public class CameraPermissionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CameraPermissionException class.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public CameraPermissionException(string message)
            : base(message)
        {
        }
    }
}