namespace Mitten.Mobile.Devices
{
    /// <summary>
    /// Represents the current permission status of a device's camera.
    /// </summary>
    public enum CameraPermission
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Identifies that a camera was not detected on the device.
        /// </summary>
        NotAvailable,

        /// <summary>
        /// Indicates that the user has not specified permissions for the camera.
        /// </summary>
        NotDetermined,

        /// <summary>
        /// The camera was restricted on the device, for example, a parent may set device
        /// wide restrictions on a phone to certain features such as the camera.
        /// </summary>
        Restricted,

        /// <summary>
        /// Identifies that the user explicitly denied access to the camera when prompted in the app.
        /// </summary>
        Denied,

        /// <summary>
        /// Identifies that the user has authorized access to the camera.
        /// </summary>
        Authorized,
    }
}