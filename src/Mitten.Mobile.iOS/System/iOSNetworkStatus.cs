using System;
using System.Net;
using Mitten.Mobile.System;
using SystemConfiguration;

namespace Mitten.Mobile.iOS.System
{
    /// <summary>
    /// Handles providing network status information for the application running in iOS.
    /// </summary>
    public class iOSNetworkStatus : INetworkStatus
    {
        private NetworkAvailability? currentAvailability;

        /// <summary>
        /// Occurs when the network availability has changed.
        /// </summary>
        public event EventHandler AvailabilityChanged = delegate { };

        /// <summary>
        /// Enables monitoring for changes in network availability.
        /// </summary>
        public void EnableMonitor()
        {
            using (NetworkReachability reachability = new NetworkReachability(new IPAddress(0)))
            {
                reachability.SetNotification(
                    flags => 
                    {
                        this.currentAvailability = null;
                        this.AvailabilityChanged(this, EventArgs.Empty);
                    });

                reachability.Schedule();
            }
        }

        /// <summary>
        /// Gets the current network availability.
        /// </summary>
        /// <returns>The current network availability.</returns>
        public NetworkAvailability GetNetworkAvailability()
        {
            if (this.currentAvailability.HasValue)
            {
                return this.currentAvailability.Value;
            }

            using (NetworkReachability reachability = new NetworkReachability(new IPAddress(0)))
            {
                NetworkReachabilityFlags flags;
                if (reachability.TryGetFlags(out flags))
                {
                    bool isReachable = iOSNetworkStatus.HasFlag(flags, NetworkReachabilityFlags.Reachable);
                    bool requireConnection = iOSNetworkStatus.HasFlag(flags, NetworkReachabilityFlags.ConnectionRequired);

                    // require connection can be true if wifi is available but the device needs to
                    // connect or authenticate through a VPN

                    if (isReachable && !requireConnection)
                    {
                        this.currentAvailability =
                            iOSNetworkStatus.HasFlag(flags, NetworkReachabilityFlags.IsWWAN)
                            ? NetworkAvailability.AvailableViaCarrierDataNetwork
                            : NetworkAvailability.AvailableViaWiFi;

                        return this.currentAvailability.Value;
                    }
                }
            }

            this.currentAvailability = NetworkAvailability.NotAvailable;
            return this.currentAvailability.Value;
        }

        private static bool HasFlag(NetworkReachabilityFlags flags, NetworkReachabilityFlags flag)
        {
            return (flags & flag) == flag;
        }
    }
}