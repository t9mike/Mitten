namespace Mitten.Mobile.System
{
    /// <summary>
    /// Defines the different types of availability for a network.
    /// </summary>
    public enum NetworkAvailability
    {
        /// <summary>
        /// Indicates that a network is currently not available.
        /// </summary>
        NotAvailable,

        /// <summary>
        /// Indicates that the network is available over the carrier network.
        /// </summary>
        AvailableViaCarrierDataNetwork,

        /// <summary>
        /// Indicates that the network is available over wifi.
        /// </summary>
        AvailableViaWiFi,
    }
}