using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mitten.Mobile.Application.AddressBook;
using Mitten.Mobile.Application.Components;
using Mitten.Mobile.Application.PushNotifications;
using Mitten.Mobile.Devices;
using Mitten.Mobile.Remote;
using Mitten.Mobile.System;
using Mitten.Mobile.ViewModels;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Handles application hosting and management which includes startup, 
    /// user verification, and access to system level information.
    /// </summary>
    public abstract class ApplicationHost
    {
        private readonly IDictionary<Type, List<Action<object>>> notificationHandlers;
        private readonly IApplicationInstanceFactory applicationInstanceFactory;

        private PushNotificationTypes knownNotificationTypes;
        private PushNotificationRegistry pushNotificationRegistry;
        private int hostThreadId;

        /// <summary>
        /// Initializes a new instance of the ApplicationHost class.
        /// </summary>
        /// <param name="createApplicationInstance">A factory method for creating application instances.</param>
        protected ApplicationHost(Func<ApplicationHost, Session, ApplicationInstance> createApplicationInstance)
            : this(new ApplicationInstanceFactory(createApplicationInstance))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationHost class.
        /// </summary>
        /// <param name="applicationInstanceFactory">An object used to create application instances.</param>
        protected ApplicationHost(IApplicationInstanceFactory applicationInstanceFactory)
        {
            Throw.IfArgumentNull(applicationInstanceFactory, nameof(applicationInstanceFactory));
            this.applicationInstanceFactory = applicationInstanceFactory;

            this.notificationHandlers = new ConcurrentDictionary<Type, List<Action<object>>>();
        }

        /// <summary>
        /// Gets an object that provides access to the data stored in the current device's address book.
        /// </summary>
        public abstract IAddressBook AddressBook { get; }

        /// <summary>
        /// Gets an object responsible for showing basic alert messages to the user.
        /// </summary>
        public abstract IApplicationAlert Alert { get; }

        /// <summary>
        /// Gets an object that provides information about the current system.
        /// </summary>
        public abstract ISystemInformation SystemInformation { get; }

        /// <summary>
        /// Gets an object that provides network connectivity status information for the current device.
        /// </summary>
        public abstract INetworkStatus NetworkStatus { get; }

        /// <summary>
        /// Gets a catalog that provides integration with software components available on the current mobile device's platform.
        /// </summary>
        public abstract IComponentCatalog Components { get; }

        /// <summary>
        /// Gets a catalog that provides low-level access to the various peripherals (secondary devices) found on a mobile device.
        /// </summary>
        public abstract IDeviceCatalog Devices { get; }

        /// <summary>
        /// Gets access to the current device settings.
        /// </summary>
        public abstract IDeviceSettings DeviceSettings { get; }

        /// <summary>
        /// Gets access to the status bar for the current device.
        /// </summary>
        public abstract IDeviceStatusBar StatusBar { get; }

        /// <summary>
        /// Gets whether or not remote notifications are enabled for the current device.
        /// </summary>
        public bool AreRemoteNotificationsEnabled => this.pushNotificationRegistry != null && this.pushNotificationRegistry.AreRemoteNotificationsEnabled;

        /// <summary>
        /// Gets the application instance for the current host.
        /// </summary>
        internal protected ApplicationInstance ApplicationInstance { get; private set; }

        /// <summary>
        /// Enables support for push notifications in the current application. 
        /// </summary>
        /// <param name="knownNotificationTypes">A collection of expected notifications that will be received by the application.</param>
        /// <param name="notificationServerRegistration">An object responsible for registering the current device to receive push notifications from a remote server.</param>
        public void EnablePushNotifications(PushNotificationTypes knownNotificationTypes, IPushNotificationServerRegistration notificationServerRegistration)
        {
            if (this.pushNotificationRegistry == null)
            {
                this.knownNotificationTypes = knownNotificationTypes;
                this.pushNotificationRegistry = this.CreatePushNotificationRegistry(notificationServerRegistration);
            }
        }

        /// <summary>
        /// Launches the initial view for the application.
        /// </summary>
        /// <param name="notification">An optional push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        /// <returns>The launch task.</returns>
        public void Launch(PushNotification notification = null)
        {
            this.Launch(null, notification);
        }

        /// <summary>
        /// Launches the initial view for the application.
        /// </summary>
        /// <param name="authenticationProcess">An optional authentication process.</param>
        /// <param name="notification">An optional push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        public void Launch(IAuthenticationProcess authenticationProcess, PushNotification notification = null)
        {
            if (this.ApplicationInstance != null)
            {
                throw new InvalidOperationException("An application instance has already been launched.");
            }

            this.hostThreadId = Environment.CurrentManagedThreadId;
            INavigation navigation = this.CreateNavigation();

            if (authenticationProcess != null)
            {
                authenticationProcess.Completed +=
                    session =>
                        this.AuthenticationComplete(
                            session,
                            navigation,
                            authenticationProcess,
                            notification);

                authenticationProcess.Launch(navigation);
            }
            else
            {
                this.LaunchApplicationInstance(Session.Anonymous, navigation, notification);
            }
        }

        /// <summary>
        /// Registers a callback that will be invoked when a push notification has been received.
        /// </summary>
        /// <param name="handler">The notification handler.</param>
        public void RegisterPushNotificationHandler<TNotification>(Action<TNotification> handler)
            where TNotification : PushNotification
        {
            Type notificationType = typeof(TNotification);

            List<Action<object>> handlers;
            if (!this.notificationHandlers.ContainsKey(notificationType))
            {
                handlers = new List<Action<object>>();
                this.notificationHandlers.Add(notificationType, handlers);
            }
            else
            {
                handlers = this.notificationHandlers[notificationType];
            }

            handlers.Add(item => handler((TNotification)item));
        }

        /// <summary>
        /// Attempts to register the current device to receive push notifications.
        /// </summary>
        public void TryRegisterDeviceForPushNotifications()
        {
            if (this.pushNotificationRegistry != null)
            {
                this.pushNotificationRegistry.RequestNotificationToken();
            }
        }

        /// <summary>
        /// Handles when a push notification has been received.
        /// </summary>
        /// <param name="notification">A dictionary containing the values for a push notification.</param>
        public void HandlePushNotificationReceived(IDictionary<string, string> notification)
        {
            this.HandlePushNotificationReceived(this.ParseNotification(notification, ignoreInvalidNotification:true));
        }

        /// <summary>
        /// Handles when a push notification has been received.
        /// </summary>
        /// <param name="notification">A push notification.</param>
        public void HandlePushNotificationReceived(PushNotification notification)
        {
            Type notificationType = notification.GetType();

            List<Action<object>> handlers;
            if (this.notificationHandlers.TryGetValue(notificationType, out handlers))
            {
                foreach (Action<object> handler in handlers)
                {
                    handler(notification);
                }
            }
        }

        /// <summary>
        /// Creates a navigation instanced used to navigate between screens in the application.
        /// </summary>
        /// <returns>A navigation instance.</returns>
        protected abstract INavigation CreateNavigation();

        /// <summary>
        /// Creates a push notification registry for the current application.
        /// </summary>
        /// <param name="notificationServerRegistration">An object responsible for registering the current device to receive push notifications from a remote server.</param>
        protected abstract PushNotificationRegistry CreatePushNotificationRegistry(IPushNotificationServerRegistration notificationServerRegistration);

        /// <summary>
        /// Invokes the specified action on the main thread.
        /// </summary>
        /// <param name="action">An action to invoke.</param>
        protected abstract void InvokeOnMainThread(Action action);

        /// <summary>
        /// Launches the initial view for the application.
        /// </summary>
        /// <param name="notification">An optional dictionary containing the values for a push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        /// <param name="ignoreInvalidNotification">True if an invalid notification should be ignored while launching, otherwise false if an exception should be thrown.</param>
        protected void Launch(IDictionary<string, string> notification = null, bool ignoreInvalidNotification = true)
        {
            this.Launch(null, this.ParseNotification(notification, ignoreInvalidNotification));
        }

        /// <summary>
        /// Launches the initial view for the application.
        /// </summary>
        /// <param name="authenticationProcess">An optional authentication process.</param>
        /// <param name="notification">An optional dictionary containing the values for a push notification if the application was launched due to the user acknowledging a notification on their device.</param>
        /// <param name="ignoreInvalidNotification">True if an invalid notification should be ignored while launching, otherwise false if an exception should be thrown.</param>
        protected void Launch(IAuthenticationProcess authenticationProcess, IDictionary<string, string> notification = null, bool ignoreInvalidNotification = true)
        {
            this.Launch(authenticationProcess, this.ParseNotification(notification, ignoreInvalidNotification));
        }

        /// <summary>
        /// Update the notification token for the current account.
        /// </summary>
        /// <param name="notificationToken">A notification token uniquely identifying this device for sending push notifications.</param>
        /// <returns>The task for the operation.</returns>
        protected Task<ServiceResult> UpdatePushNotificationToken(string notificationToken)
        {
            if (this.pushNotificationRegistry != null)
            {
                if (this.ApplicationInstance == null)
                {
                    throw new SessionExpiredException();
                }

                return
                    this.pushNotificationRegistry.UpdateNotificationToken(
                        this.ApplicationInstance.CurrentSession.Account,
                        notificationToken);
            }

            throw new InvalidOperationException("Push notifications are not enabled for the current application.");
        }

        /// <summary>
        /// Handles when a new application instance has failed to initialize.
        /// </summary>
        /// <param name="navigation">A navigation object for the current application.</param>
        /// <param name="result">Result.</param>
        protected virtual void HandleFailedApplicationInitialization(INavigation navigation, ServiceResult result)
        {
            throw new ApplicationInitializationException("Failed to initialize an application instance with result (" + result.ResultCode + "): " + result.FailureDetails + ".");
        }

        private async void AuthenticationComplete(
            Session session,
            INavigation navigation,
            IAuthenticationProcess authenticationProcess,
            PushNotification notification)
        {
            if (Environment.CurrentManagedThreadId != this.hostThreadId)
            {
                this.InvokeOnMainThread(() => this.AuthenticationComplete(session, navigation, authenticationProcess, notification));
                return;
            }

            this.ApplicationInstance = this.applicationInstanceFactory.Create(this, session);
            ServiceResult result = await this.ApplicationInstance.InitializeUserSession();

            if (result.ResultCode == ServiceResultCode.Success)
            {
                this.ApplicationInstance.SignedOut +=
                    () =>
                    {
                        this.ApplicationInstance = null;
                        authenticationProcess.LaunchFromSignOut(navigation);
                    };

                this.ApplicationInstance.Launch(navigation, notification);
            }
            else
            {
                this.ApplicationInstance = null;
                this.HandleFailedApplicationInitialization(navigation, result);
            }
        }
         
        private void LaunchApplicationInstance(Session session, INavigation navigation, PushNotification notification)
        {
            this.ApplicationInstance = this.applicationInstanceFactory.Create(this, session);
            this.ApplicationInstance.Launch(navigation, notification);
        }

        private PushNotification ParseNotification(IDictionary<string, string> notification, bool ignoreInvalidNotification)
        {
            PushNotification pushNotification = null;

            if (this.knownNotificationTypes != null && notification != null)
            {
                PushNotificationParseResult result = PushNotification.FromDictionary(this.knownNotificationTypes, notification);
                if (result.Result == PushNotificationParseResult.ResultCode.Success)
                {
                    pushNotification = result.Notification;
                }
                else if (!ignoreInvalidNotification)
                {
                    throw new ArgumentException("Invalid notification data, failed to parse with result (" + result.Result + ").", nameof(notification));
                }
            }

            return pushNotification;
        }

        private class ApplicationInstanceFactory : IApplicationInstanceFactory
        {
            private readonly Func<ApplicationHost, Session, ApplicationInstance> create;

            public ApplicationInstanceFactory(Func<ApplicationHost, Session, ApplicationInstance> create)
            {
                this.create = create;
            }

            public ApplicationInstance Create(ApplicationHost host, Session session)
            {
                return this.create(host, session);
            }
        }
    }
}