namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Defines an component that provides access to a device's in order to make phone calls.
    /// </summary>
    public interface IPhoneComponent
    {
        /// <summary>
        /// Determines whether or not the current device supports making phone calls.
        /// </summary>
        /// <returns>True if the device supports making phone calls.</returns>
        bool CanMakePhoneCall();

        /// <summary>
        /// Makes a phone call to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber">A phone number to call.</param>
        void MakePhoneCall(string phoneNumber);
    }
}