using Foundation;
using Mitten.Mobile.Application.Components;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Provides access to make phone calls on an iOS device.
    /// </summary>
    public class iOSPhoneComponent : IPhoneComponent
    {
        private static class Constants
        {
            public const string PhoneUrl = "tel://";
        }

        /// <summary>
        /// Determines whether or not the current device supports making phone calls.
        /// </summary>
        /// <returns>True if the device supports making phone calls.</returns>
        public bool CanMakePhoneCall()
        {
            NSUrl url = new NSUrl(Constants.PhoneUrl);
            return UIApplication.SharedApplication.CanOpenUrl(url);
        }

        /// <summary>
        /// Makes a phone call to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber">A phone number to call.</param>
        public void MakePhoneCall(string phoneNumber)
        {
            NSUrl url = new NSUrl(Constants.PhoneUrl + phoneNumber);
            UIApplication.SharedApplication.OpenUrl(url);
        }
    }
}
