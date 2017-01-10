using System.Collections.Generic;
using System.Linq;
using Foundation;
using Mitten.Mobile.Identity;
using Security;

namespace Mitten.Mobile.iOS.Identity
{
    /// <summary>
    /// Supports storing account details using the iOS key chain.
    /// </summary>
    public class KeyChainAccountStore : ILocalAccountStore
    {
        private readonly string applicationId;

        /// <summary>
        /// Initializes a new instance of the KeyChainAccountStore class.
        /// </summary>
        /// <param name="applicationId">Am id used to identify the current app in the key chain.</param>
        public KeyChainAccountStore(string applicationId)
        {
            Throw.IfArgumentNullOrWhitespace(applicationId, nameof(applicationId));
            this.applicationId = applicationId;
        }

        /// <summary>
        /// Saves the specified account credentials.
        /// </summary>
        /// <param name="accountCredentials">The user credentials to save.</param>
        public void Save(AccountCredentials accountCredentials)
        {
            Throw.IfArgumentNull(accountCredentials, nameof(accountCredentials));

            this.DeleteAccountCredentials();

            SecRecord newRecord = new SecRecord(SecKind.GenericPassword);

            newRecord.Service = this.applicationId;
            newRecord.Generic = NSData.FromString(accountCredentials.Serialize(), NSStringEncoding.UTF8);

            SecKeyChain.Add(newRecord);
        }

        /// <summary>
        /// Fetches the currently saved set of account credentials or null if no credentials have been saved.
        /// </summary>
        /// <returns>The user credentials.</returns>
        public AccountCredentials GetAccountCredentials()
        {
            IEnumerable<SecRecord> records = this.GetRecords();

            if (records.Any())
            {
                string serializedData = NSString.FromData(records.First().Generic, NSStringEncoding.UTF8);
                return AccountCredentials.TryDeserialize(serializedData);
            }

            return null;
        }

        /// <summary>
        /// Deletes any account credentials that are stored.
        /// </summary>
        public void DeleteAccountCredentials()
        {
            SecRecord recordToDelete = new SecRecord(SecKind.GenericPassword);
            recordToDelete.Service = this.applicationId;

            SecKeyChain.Remove(recordToDelete);
        }

        private IEnumerable<SecRecord> GetRecords()
        {
            SecRecord query = new SecRecord(SecKind.GenericPassword);
            query.Service = this.applicationId;

            SecStatusCode result;
            SecRecord[] records = SecKeyChain.QueryAsRecord(query, int.MaxValue, out result);

            if (result == SecStatusCode.Success)
            {
                return records;
            }

            return new SecRecord[0];
        }
    }
}
