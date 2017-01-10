namespace Mitten.Mobile.Devices
{
    /// <summary>
    /// Provides access to common peripherals (secondary devices) found on a mobile device. 
    /// </summary>
    public interface IDeviceCatalog
    {
        /// <summary>
        /// Gets the camera device.
        /// </summary>
        /// <returns>The camera.</returns>
        ICameraDevice GetCamera();
    }
}
