namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Defines a repository for notification accounts.
    /// </summary>
    /// <typeparam name="TKey">Identifies the data type for the account's id.</typeparam>
    public interface INotificationAccountRepository<TKey>
    {
        /// <summary>
        /// Creates a notification account.
        /// </summary>
        /// <param name="notificationAccount">The account to create.</param>
        void CreateAccount(NotificationAccount<TKey> notificationAccount);

        /// <summary>
        /// Updates an existing notification account.
        /// </summary>
        /// <param name="notificationAccount">The account to update.</param>
        void UpdateAccount(NotificationAccount<TKey> notificationAccount);

        /// <summary>
        /// Deletes a notification account.
        /// </summary>
        /// <param name="accountId">The id of the account to delete.</param>
        void DeleteAccount(TKey accountId);

        /// <summary>
        /// Gets a notification account.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <returns>The account details or null if the account does not exist.</returns>
        NotificationAccount<TKey> GetAccount(TKey accountId);
    }
}
