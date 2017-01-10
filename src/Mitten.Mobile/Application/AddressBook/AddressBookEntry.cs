using System;
using System.Collections.Generic;

namespace Mitten.Mobile.Application.AddressBook
{
    /// <summary>
    /// Contains data for an entry in the mobile device's address book.
    /// </summary>
    public class AddressBookEntry
    {
        /// <summary>
        /// Gets or sets the unique id assigned by an external source if the contact was imported from another application.
        /// </summary>
        public string ExternalContactId { get; set; }

        /// <summary>
        /// Gets or sets the external source if this contact was imported from another application.
        /// </summary>
        public string ExternalContactSource { get; set; }

        /// <summary>
        /// Gets or sets the date and time the external source modified the contact.
        /// </summary>
        public DateTime? ExternalModificationDate { get; set; }

        /// <summary>
        /// Gets or sets whether or not the address book entry is a company, otherwise false if it is for a person.
        /// </summary>
        public bool IsCompany { get; set; }

        /// <summary>
        /// Gets or sets the image for the person.
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the company the person works for.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets a list of physical mailing addresses for the contact.
        /// </summary>
        public IEnumerable<KeyValuePair<string, Address>> Addresses { get; set; }

        /// <summary>
        /// Gets or sets a list of email addresses for the contact.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> EmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets a list of phone numbers for the contact.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> PhoneNumbers { get; set; }

        /// <summary>
        /// Gets whether or not the person's name for this address book entry contains the given string.
        /// </summary>
        /// <param name="namePart">The name part to check.</param>
        /// <returns>True if the name contains the specified name part string, otherwise false.</returns>
        public bool DoesNameContain(string namePart)
        {
            return
                this.Contains(this.FirstName, namePart) ||
                this.Contains(this.LastName, namePart);
        }

        private bool Contains(string text, string subString)
        {
            return
                !string.IsNullOrWhiteSpace(text) &&
                text.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}