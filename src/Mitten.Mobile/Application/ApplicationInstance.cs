using System;
using System.Threading.Tasks;
using Mitten.Mobile.Application.PushNotifications;
using Mitten.Mobile.Remote;
using Mitten.Mobile.ViewModels;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Represents a running instance of the application within the context of a single user session.
    /// </summary>
    public abstract class ApplicationInstance
    {
        private Session session;

        /// <summary>
        /// Initializes a new instance of the ApplicationInstance class.
        /// </summary>
        /// <param name="host">The parent host for the application.</param>
        /// <param name="session">A user session.</param>
        protected ApplicationInstance(ApplicationHost host, Session session)
        {
            Throw.IfArgumentNull(host, nameof(host));
            Throw.IfArgumentNull(session, nameof(session));

            this.Host = host;
            this.session = session;
        }

        /// <summary>
        /// Occurs when the current user session has signed out.
        /// </summary>
        public event Action SignedOut = delegate { };

        /// <summary>
        /// Gets the current user session.
        /// </summary>
        public Session CurrentSession
        {
            get
            {
                if (this.session == null)
                {
                    throw new SessionExpiredException();
                }

                return this.session;
            }
        }

        /// <summary>
        /// Gets the application host.
        /// </summary>
        public ApplicationHost Host { get; }

        /// <summary>
        /// Signs out the current user if the current session is not anonymous.
        /// </summary>
        protected void SignOut()
        {
            if (this.session == null)
            {
                throw new SessionExpiredException();
            }

            if (this.session.IsAnonymous)
            {
                throw new InvalidOperationException("Cannot sign out of an anonymous session.");
            }

            this.session = null;
            this.SignedOut();
        }

        /// <summary>
        /// Initializes the current application instance for a user session after authentication and prior to launching the instance.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        internal protected virtual Task<ServiceResult> InitializeUserSession()
        {
            return Task.FromResult(ServiceResult.Success());
        }

        /// <summary>
        /// Launches into the first screen of the application instance.
        /// </summary>
        /// <param name="navigation">The navigation object used for navigation.</param>
        /// <param name="notification">An optional push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        internal protected abstract void Launch(INavigation navigation, PushNotification notification);
    }

    /// <summary>
    /// A concrete application instance that will launch into the screen for the specified view model.
    /// </summary>
    public class ApplicationInstance<THomeViewModel> : ApplicationInstance
        where THomeViewModel : ViewModel<ApplicationInstance<THomeViewModel>>
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationInstance class.
        /// </summary>
        /// <param name="host">The parent host for the application.</param>
        /// <param name="session">A user session.</param>
        public ApplicationInstance(ApplicationHost host, Session session)
            : base(host, session)
        {
        }

        /// <summary>
        /// Launches into the first screen of the application instance.
        /// </summary>
        /// <param name="navigation">The navigation object used for navigation.</param>
        /// <param name="notification">An optional push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        internal protected override void Launch(INavigation navigation, PushNotification notification)
        {
            navigation.NavigateTo<THomeViewModel>(new NavigationOptions(notification, PresentationType.Root));
        }
    }
}