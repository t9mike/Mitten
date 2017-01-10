using Mitten.Mobile.Devices;

namespace Mitten.Mobile.iOS.Devices
{
    /// <summary>
    /// Provides access to common peripherals (secondary devices) found on an iOS mobile device. 
    /// </summary>
    public class iOSDeviceCatalog : IDeviceCatalog
    {
        /// <summary>
        /// Gets the camera device.
        /// </summary>
        /// <returns>The camera.</returns>
        public ICameraDevice GetCamera()
        {
            return new CameraDevice();
        }
    }
}
