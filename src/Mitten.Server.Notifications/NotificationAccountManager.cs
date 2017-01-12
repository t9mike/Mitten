using System;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Handles managing notification accounts.
    /// </summary>
    /// <typeparam name="TKey">Identifies the data type for the account's id.</typeparam>
    public class NotificationAccountManager<TKey>
    {
        private readonly INotificationAccountRepository<TKey> repository;

        /// <summary>
        /// Initializes a new instance of the NotificationAccountManager class.
        /// </summary>
        /// <param name="repository">A notification repository.</param>
        public NotificationAccountManager(INotificationAccountRepository<TKey> repository)
        {
            Throw.IfArgumentNull(repository, nameof(repository));
            this.repository = repository;
        }

        /// <summary>
        /// Gets the account with the specified id.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>A notification account.</returns>
        public NotificationAccount<TKey> GetAccount(TKey accountId)
        {
            NotificationAccount<TKey> account = this.repository.GetAccount(accountId);

            if (account == null)
            {
                throw new NotificationAccountNotFoundException(accountId);
            }

            return account;
        }

        /// <summary>
        /// Attempts to get the account with the specified id.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>A notification account or null if the account does not exist.</returns>
        public NotificationAccount<TKey> TryGetAccount(TKey accountId)
        {
            return this.repository.GetAccount(accountId);
        }

        /// <summary>
        /// Creates a new account for receiving notifications.
        /// </summary>
        /// <param name="account">An account to create.</param>
        public void CreateAccount(NotificationAccount<TKey> account)
        {
            Throw.IfArgumentNull(account, nameof(account));
            this.repository.CreateAccount(account);
        }

        /// <summary>
        /// Updates an existing notificaitons account.
        /// </summary>
        /// <param name="account">An account to update.</param>
        public void UpdateAccount(NotificationAccount<TKey> account)
        {
            Throw.IfArgumentNull(account, nameof(account));
            this.repository.UpdateAccount(account);
        }

        /// <summary>
        /// Deletes a notification account.
        /// </summary>
        /// <param name="accountId">An account id.</param>
        public void DeleteAccount(TKey accountId)
        {
            this.repository.DeleteAccount(accountId);
        }

        /// <summary>
        /// Attempts to disable a specific type of notifications for an account.
        /// </summary>
        /// <param name="accountId">The id for the account whose notifications to disable.</param>
        /// <param name="notificationType">The type of notification to disable.</param>
        /// <returns>True if the notifications were disabled otherwise false if an account notifications aren't currently enabled.</returns>
        public bool TryDisableNotifications(TKey accountId, NotificationType notificationType)
        {
            if (notificationType != NotificationType.Push)
            {
                throw new ArgumentException("Notifications of type (" + notificationType + " ) are not currently supported.", nameof(notificationType));
            }

            return
                this.TryUpdateAccount(
                    accountId,
                    account => account.DisablePushNotifications());
        }

        /// <summary>
        /// Registers a new mobile device with the specified account.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        /// <param name="mobileDevice">A mobile device to register.</param>
        public void RegisterMobileDevice(TKey accountId, MobileDevice mobileDevice)
        {
            Throw.IfArgumentNull(mobileDevice, nameof(mobileDevice));

            NotificationAccount<TKey> account = this.GetAccount(accountId);
            account.AddMobileDevice(mobileDevice);

            this.repository.UpdateAccount(account);
        }

        /// <summary>
        /// Enables push notifications for an account.
        /// </summary>
        /// <param name="accountId">The id of the account who is using the device.</param>
        /// <param name="deviceId">The device to enable notifications for.</param>
        /// <param name="pushNotificationToken">A token associated with the device to receive notifications.</param>
        public void EnablePushNotifications(TKey accountId, string deviceId, string pushNotificationToken)
        {
            NotificationAccount<TKey> account = this.GetAccount(accountId);
            account.EnablePushNotifications(deviceId, pushNotificationToken);
        }

        private bool TryUpdateAccount(TKey accountId, Action<NotificationAccount<TKey>> modifyAccount)
        {
            NotificationAccount<TKey> account = this.repository.GetAccount(accountId);

            if (account != null)
            {
                modifyAccount(account);
                this.repository.UpdateAccount(account);
                return true;
            }

            return false;
        }
    }
}