using System;
using System.Threading.Tasks;
using Mitten.Mobile.Application;
using Mitten.Mobile.Remote;

namespace Mitten.Mobile.Identity
{
    /// <summary>
    /// Handles authenticating accounts.
    /// </summary>
    public interface IAuthenticationManager
    {
        /// <summary>
        /// Occurs when a user has successfully signed in.
        /// </summary>
        event Action<AccountCredentials, Session> SignedIn;

        /// <summary>
        /// Signs a user into the system using the specified account credentials.
        /// </summary>
        /// <param name="accountCredentials">Account credentials.</param>
        /// <returns>The result of a user sign in.</returns>
        Task<ServiceResult> SignIn(AccountCredentials accountCredentials);

        /// <summary>
        /// signs the user out of the system.
        /// </summary>
        /// <param name="session">The session to sign out of.</param>
        /// <returns>The result of the operation.</returns>
        Task<ServiceResult> SignOut(Session session);
    }
}