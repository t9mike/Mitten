namespace Mitten.Mobile.Devices
{
    /// <summary>
    /// Defines the camera's flash mode.
    /// </summary>
    public enum CameraFlashMode
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Indicates that the flash is not available.
        /// </summary>
        NotAvailable,

        /// <summary>
        /// The flash is on.
        /// </summary>
        On,

        /// <summary>
        /// The flash is set to auto.
        /// </summary>
        Auto,

        /// <summary>
        /// The flash is off.
        /// </summary>
        Off,
    }
}