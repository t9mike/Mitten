namespace Mitten.Mobile.Application.AddressBook
{
    /// <summary>
    /// Represents the current permission status of a device's address book.
    /// </summary>
    public enum AddressBookPermission
    {
        /// <summary>
        /// Indicates that the status could not be determined.
        /// </summary>
        NotDetermined,

        /// <summary>
        /// The address book was restricted on the device and the permission cannot be changed by the user.
        /// </summary>
        Restricted,

        /// <summary>
        /// Identifies that the user explicitly denied access to the address book when prompted in the app.
        /// </summary>
        Denied,

        /// <summary>
        /// Identifies that the user has authorized access to the address book.
        /// </summary>
        Authorized,
    }
}