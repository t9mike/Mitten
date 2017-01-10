using System;
using Mitten.Mobile.Identity;
using Mitten.Mobile.Remote;
using Mitten.Mobile.ViewModels;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// A basic authentication process that supports auto-sign.
    /// </summary>
    /// <typeparam name="TAutoSignInViewModel">The view model to display when auto-signing in a user.</typeparam>
    /// <typeparam name="TIntroViewModel">The view model to display to the user.</typeparam>
    public class AuthenticationProcess<TAutoSignInViewModel, TIntroViewModel> : IAuthenticationProcess
        where TAutoSignInViewModel : ViewModel
        where TIntroViewModel : ViewModel
    {
        private readonly IAuthenticationManager authenticationManager;
        private readonly ILocalAccountStore localAccountStore;

        /// <summary>
        /// Initializes a new instance of the AuthenticationProcess class.
        /// </summary>
        /// <param name="authenticationManager">An authentication manager.</param>
        /// <param name="localAccountStore">A local store used to save account credentials locally on the device.</param>
        public AuthenticationProcess(IAuthenticationManager authenticationManager, ILocalAccountStore localAccountStore)
        {
            Throw.IfArgumentNull(authenticationManager, nameof(authenticationManager));
            Throw.IfArgumentNull(localAccountStore, nameof(localAccountStore));

            this.authenticationManager = authenticationManager;
            this.localAccountStore = localAccountStore;

            this.authenticationManager.SignedIn +=
                (credentials, session) =>
                {
                    this.localAccountStore.Save(credentials);
                    this.Completed(session);
                };
        }

        /// <summary>
        /// Occurs when the authentication process has completed and a user has been signed in.
        /// </summary>
        public event Action<Session> Completed = delegate { };

        /// <summary>
        /// Launches into the authentication process.
        /// </summary>
        /// <param name="navigation">A navigation object used to navigate the screens during the authorization process.</param>
        public void Launch(INavigation navigation)
        {
            AccountCredentials credentials = this.localAccountStore.GetAccountCredentials();

            if (credentials != null)
            {
                navigation.NavigateTo<TAutoSignInViewModel>(new NavigationOptions(null, PresentationType.Root));
                this.AutoSignIn(navigation, credentials);
            }
            else
            {
                this.NavigateToIntroView(navigation);
            }
        }

        /// <summary>
        /// Handles launching back into the authentication process after a user has signed out of the application.
        /// </summary>
        /// <param name="navigation">An object used to navigate the screens during the authorization process.</param>
        public void LaunchFromSignOut(INavigation navigation)
        {
            this.localAccountStore.DeleteAccountCredentials();
            this.NavigateToIntroView(navigation);
        }

        private async void AutoSignIn(INavigation navigation, AccountCredentials credentials)
        {
            ServiceResult result = await this.authenticationManager.SignIn(credentials);
            if (result.ResultCode != ServiceResultCode.Success)
            {
                this.NavigateToIntroView(navigation);
            }
        }

        private void NavigateToIntroView(INavigation navigation)
        {
            navigation.NavigateTo<TIntroViewModel>(new NavigationOptions(this.authenticationManager, PresentationType.Root));
        }
    }
}