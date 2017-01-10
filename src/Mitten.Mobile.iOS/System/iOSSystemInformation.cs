using System;
using Foundation;
using Mitten.Mobile.System;
using ObjCRuntime;

namespace Mitten.Mobile.iOS.System
{
    /// <summary>
    /// Provides information about the application and iOS system for the current device.
    /// </summary>
    public class iOSSystemInformation : ISystemInformation
    {
        /// <summary>
        /// Gets the current version of the executing application.
        /// </summary>
        /// <returns>The application version.</returns>
        public string GetAppVersion()
        {
            NSString version = (NSString)NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            return version.ToString();
        }

        /// <summary>
        /// Gets the current runtime environment.
        /// </summary>
        /// <returns>The current runtime environment.</returns>
        public RuntimeEnvironment GetRuntimeEnvironment()
        {
            if (Runtime.Arch == Arch.DEVICE)
            {
                return RuntimeEnvironment.Device;
            }

            if (Runtime.Arch == Arch.SIMULATOR)
            {
                return RuntimeEnvironment.Simulator;
            }

            throw new InvalidOperationException("Unknown runtime environment (" + Runtime.Arch + ").");
        }
    }
}