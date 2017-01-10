using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AddressBook;
using Foundation;
using Mitten.Mobile.Application.AddressBook;

namespace Mitten.Mobile.iOS.Application.AddressBook
{
    /// <summary>
    /// Provides access to the iOS address book.
    /// </summary>
    public class iOSAddressBook : IAddressBook
    {
        private static class Constants
        {
            public static readonly DateTime ReferenceDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            public const string ExternalContactSource = "iOS";
            public const string LabelTrimStart = "_$!<";
            public const string LabelTrimEnd = ">!$_";
        }

        private ABAddressBook addressBook;

        /// <summary>
        /// Gets the current authorization permissions for the address book.
        /// </summary>
        /// <returns>The current permission.</returns>
        public AddressBookPermission GetPermission()
        {
            ABAuthorizationStatus status = this.GetAuthorizationStatus();

            if (status == ABAuthorizationStatus.Authorized)
            {
                return AddressBookPermission.Authorized;
            }

            if (status == ABAuthorizationStatus.Denied)
            {
                return AddressBookPermission.Denied;
            }

            if (status == ABAuthorizationStatus.NotDetermined)
            {
                return AddressBookPermission.NotDetermined;
            }

            if (status == ABAuthorizationStatus.Restricted)
            {
                return AddressBookPermission.Restricted;
            }

            throw new InvalidOperationException("Unexpected ABAuthorizationStatus (" + status + ").");
        }

        /// <summary>
        /// Requests access to the device's address book.
        /// </summary>
        /// <returns>True if access was granted, otherwise false.</returns>
        public Task<bool> RequestAccess()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            this.GetAddressBook().RequestAccess((result, error) => taskCompletionSource.SetResult(result));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Gets a list of all contacts from the device's address book.
        /// </summary>
        /// <returns>A list of contacts from address book.</returns>
        public Task<IEnumerable<AddressBookEntry>> GetAllContactsFromAddressBook()
        {
            return
                Task.Factory.StartNew(
                    () =>
                        this.GetAddressBook()
                            .GetPeople()
                            .Where(item => item.Type == ABRecordType.Person)
                            .Select(this.ConvertFromAddressBook));
        }

        private AddressBookEntry ConvertFromAddressBook(ABPerson from)
        {
            AddressBookEntry entry = new AddressBookEntry();

            entry.ExternalContactId = from.Id.ToString();
            entry.ExternalContactSource = Constants.ExternalContactSource;
            entry.IsCompany = this.IsCompany(from);
            entry.FirstName = from.FirstName;
            entry.LastName = from.LastName;
            entry.CompanyName = from.Organization;
            entry.ExternalModificationDate = this.ToDateTime(from.ModificationDate);

            if (from.HasImage)
            {
                NSData imageData = from.GetImage(ABPersonImageFormat.OriginalSize);
                byte[] image = new byte[imageData.Length];

                Marshal.Copy(imageData.Bytes, image, 0, Convert.ToInt32(imageData.Length));

                entry.Image = image;
            }

            List<KeyValuePair<string, Address>> addresses = new List<KeyValuePair<string, Address>>();
            foreach (ABMultiValueEntry<PersonAddress> address in from.GetAllAddresses())
            {
                Address newAddress = new Address();

                newAddress.Street = address.Value.Street;
                newAddress.City = address.Value.City;
                newAddress.Province = address.Value.State;
                newAddress.PostalCode = address.Value.Zip;
                newAddress.Country = address.Value.Country;

                addresses.Add(new KeyValuePair<string, Address>(this.NormalizeLabel(address.Label), newAddress));
            }

            entry.Addresses = addresses;

            List<KeyValuePair<string, string>> emailAddresses = new List<KeyValuePair<string, string>>();
            foreach (ABMultiValueEntry<string> emailAddress in from.GetEmails())
            {
                emailAddresses.Add(new KeyValuePair<string, string>(this.NormalizeLabel(emailAddress.Label), emailAddress.Value));
            }

            entry.EmailAddresses = emailAddresses;

            List<KeyValuePair<string, string>> phoneNumbers = new List<KeyValuePair<string, string>>();
            foreach (ABMultiValueEntry<string> phoneNumber in from.GetPhones())
            {
                phoneNumbers.Add(new KeyValuePair<string, string>(this.NormalizeLabel(phoneNumber.Label), phoneNumber.Value));
            }

            entry.PhoneNumbers = phoneNumbers;

            return entry;
        }

        private bool IsCompany(ABPerson person)
        {
            if (person.PersonKind == ABPersonKind.Organization)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(person.Organization))
            {
                return
                    string.IsNullOrWhiteSpace(person.FirstName) &&
                    string.IsNullOrWhiteSpace(person.LastName);
            }

            return false;
        }

        private ABAddressBook GetAddressBook()
        {
            if (this.addressBook == null)
            {
                NSError error;
                this.addressBook = ABAddressBook.Create(out error);

                if (addressBook == null)
                {
                    throw new InvalidOperationException("Failed to create an AddressBook with error (" + error + ").");
                }
            }

            return this.addressBook;
        }

        private ABAuthorizationStatus GetAuthorizationStatus()
        {
            return ABAddressBook.GetAuthorizationStatus();
        }

        private string NormalizeLabel(string label)
        {
            if (!string.IsNullOrWhiteSpace(label))
            {
                return
                    label
                    .TrimStart(Constants.LabelTrimStart.ToCharArray())
                    .TrimEnd(Constants.LabelTrimEnd.ToCharArray())
                    .ToLowerInvariant();
            }

            return label;
        }

        private DateTime ToDateTime(NSDate nsDate)
        {
            return Constants.ReferenceDate.AddSeconds(nsDate.SecondsSinceReferenceDate);
        }
    }
}