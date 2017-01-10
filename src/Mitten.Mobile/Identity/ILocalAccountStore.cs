namespace Mitten.Mobile.Identity
{
    /// <summary>
    /// Defines an interface for saving and loading account details stored locally on the device.
    /// </summary>
    public interface ILocalAccountStore
    {
        /// <summary>
        /// Saves the specified account credentials.
        /// </summary>
        /// <param name="accountCredentials">The user credentials to save.</param>
        void Save(AccountCredentials accountCredentials);

        /// <summary>
        /// Fetches the currently saved set of account credentials or null if no credentials have been saved.
        /// </summary>
        /// <returns>The user credentials.</returns>
        AccountCredentials GetAccountCredentials();

        /// <summary>
        /// Deletes any account credentials that are stored.
        /// </summary>
        void DeleteAccountCredentials();
    }
}