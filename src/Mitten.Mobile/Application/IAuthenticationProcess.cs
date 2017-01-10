using System;
using Mitten.Mobile.ViewModels;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Handles the authentication process for an application.
    /// </summary>
    public interface IAuthenticationProcess
    {
        /// <summary>
        /// Occurs when the authentication process has completed and a user has been signed in.
        /// </summary>
        event Action<Session> Completed;

        /// <summary>
        /// Launches into the authentication process.
        /// </summary>
        /// <param name="navigation">An object used to navigate the screens during the authorization process.</param>
        void Launch(INavigation navigation);

        /// <summary>
        /// Handles launching back into the authentication process after a user has signed out of the application.
        /// </summary>
        /// <param name="navigation">An object used to navigate the screens during the authorization process.</param>
        void LaunchFromSignOut(INavigation navigation);
    }
}