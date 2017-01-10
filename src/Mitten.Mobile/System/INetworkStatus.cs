using System;

namespace Mitten.Mobile.System
{
    /// <summary>
    /// A contract for an object that provides network connectivity status.
    /// </summary>
    public interface INetworkStatus
    {
        /// <summary>
        /// Occurs when the network availability has changed.
        /// </summary>
        event EventHandler AvailabilityChanged;

        /// <summary>
        /// Gets the current network availability.
        /// </summary>
        /// <returns>The current network availability.</returns>
        NetworkAvailability GetNetworkAvailability();

        /// <summary>
        /// Enables monitoring for changes in network availability.
        /// </summary>
        void EnableMonitor();
    }
}