using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitten.Mobile.Application.AddressBook
{
    /// <summary>
    /// Provides access to the data in a device's address book.
    /// </summary>
    public interface IAddressBook
    {
        /// <summary>
        /// Gets the current authorization permissions for the address book.
        /// </summary>
        /// <returns>The current permission.</returns>
        AddressBookPermission GetPermission();

        /// <summary>
        /// Requests access to the device's address book.
        /// </summary>
        /// <returns>True if access was granted, otherwise false.</returns>
        Task<bool> RequestAccess();

        /// <summary>
        /// Gets a list of all contacts from the device's address book.
        /// </summary>
        /// <returns>A list of contacts from address book.</returns>
        Task<IEnumerable<AddressBookEntry>> GetAllContactsFromAddressBook();
    }
}