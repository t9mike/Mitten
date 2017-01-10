using System;
using System.Reflection;
using Foundation;
using Mitten.Mobile.Application;
using Mitten.Mobile.Application.AddressBook;
using Mitten.Mobile.Application.Components;
using Mitten.Mobile.Application.PushNotifications;
using Mitten.Mobile.Devices;
using Mitten.Mobile.iOS.Application.AddressBook;
using Mitten.Mobile.iOS.Application.Components;
using Mitten.Mobile.iOS.Application.PushNotifications;
using Mitten.Mobile.iOS.Devices;
using Mitten.Mobile.iOS.System;
using Mitten.Mobile.iOS.ViewControllers;
using Mitten.Mobile.System;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.Application
{
    /// <summary>
    /// Handles hosting an iOS application.
    /// </summary>
    public class iOSApplicationHost : ApplicationHost
    {
        private Lazy<iOSAddressBook> addressBook;
        private Lazy<iOSApplicationAlert> alert;
        private Lazy<iOSSystemInformation> systemInformation;
        private Lazy<iOSNetworkStatus> networkStatus;
        private Lazy<iOSComponentCatalog> componentCatalog;
        private Lazy<iOSDeviceCatalog> deviceCatalog;
        private Lazy<iOSDeviceSettings> deviceSettings;
        private Lazy<iOSDeviceStatusBar> statusBar;

        private readonly UIApplicationDelegate appDelegate;
        private ViewControllerTypes viewControllerTypes;

        /// <summary>
        /// Initializes a new instance of the iOSApplicationHost class.
        /// </summary>
        /// <param name="appDelegate">The application delegate hosting the iOS app.</param>
        /// <param name="createApplicationInstance">A factory method for creating application instances.</param>
        public iOSApplicationHost(
            UIApplicationDelegate appDelegate,
            Func<ApplicationHost, Session, ApplicationInstance> createApplicationInstance)
            : this(appDelegate, null, createApplicationInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the iOSApplicationHost class.
        /// </summary>
        /// <param name="appDelegate">The application delegate hosting the iOS app.</param>
        /// <param name="viewControllerTypes">
        /// A collection of view controllers and their associated view models used by the application. 
        /// If null, the host will scan the calling assembly for view controllers.
        /// </param>
        /// <param name="createApplicationInstance">A factory method for creating application instances.</param>
        public iOSApplicationHost(
            UIApplicationDelegate appDelegate,
            ViewControllerTypes viewControllerTypes,
            Func<ApplicationHost, Session, ApplicationInstance> createApplicationInstance)
            : base(createApplicationInstance)
        {
            this.appDelegate = appDelegate;
            this.viewControllerTypes = viewControllerTypes;
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the iOSApplicationHost class.
        /// </summary>
        /// <param name="appDelegate">The application delegate hosting the iOS app.</param>
        /// <param name="applicationInstanceFactory">An object used to create application instances.</param>
        public iOSApplicationHost(
            UIApplicationDelegate appDelegate,
            IApplicationInstanceFactory applicationInstanceFactory)
            : this(appDelegate, null, applicationInstanceFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the iOSApplicationHost class.
        /// </summary>
        /// <param name="appDelegate">The application delegate hosting the iOS app.</param>
        /// <param name="viewControllerTypes">
        /// A collection of view controllers and their associated view models used by the application. 
        /// If null, the host will scan the calling assembly for view controllers.
        /// </param>
        /// <param name="applicationInstanceFactory">An object used to create application instances.</param>
        public iOSApplicationHost(
            UIApplicationDelegate appDelegate, 
            ViewControllerTypes viewControllerTypes,
            IApplicationInstanceFactory applicationInstanceFactory)
            : base(applicationInstanceFactory)
        {
            this.appDelegate = appDelegate;
            this.viewControllerTypes = viewControllerTypes;
            this.Initialize();
        }

        /// <summary>
        /// Gets an object that provides access to the data stored in the current device's address book.
        /// </summary>
        public override IAddressBook AddressBook => this.addressBook.Value;

        /// <summary>
        /// Gets an object responsible for showing basic alert messages to the user.
        /// </summary>
        public override IApplicationAlert Alert => this.alert.Value;

        /// <summary>
        /// Gets an object that provides information about the current system.
        /// </summary>
        public override ISystemInformation SystemInformation => this.systemInformation.Value;

        /// <summary>
        /// Gets an object that provides network connectivity status information for the current device.
        /// </summary>
        public override INetworkStatus NetworkStatus => this.networkStatus.Value;

        /// <summary>
        /// Gets a catalog that provides integration with software components available on the current mobile device's platform.
        /// </summary>
        public override IComponentCatalog Components => this.componentCatalog.Value;

        /// <summary>
        /// Gets a catalog that provides low-level access to the various peripherals (secondary devices) found on a mobile device.
        /// </summary>
        public override IDeviceCatalog Devices => this.deviceCatalog.Value;

        /// <summary>
        /// Gets access to the current device settings.
        /// </summary>
        public override IDeviceSettings DeviceSettings => this.deviceSettings.Value;

        /// <summary>
        /// Gets access to the status bar for the current device.
        /// </summary>
        public override IDeviceStatusBar StatusBar => this.statusBar.Value;

        /// <summary>
        /// Launches the initial view for the application asynchronously.
        /// </summary>
        /// <param name="launchOptions">
        /// The launch options for an iOS application containing the data for a notification if the app was launched from the user responding to a notification alert.
        /// The dicationary will be ignored if it is empty or null.
        /// </param>
        /// <param name="ignoreInvalidNotification">True if an invalid notification should be ignored while launching, otherwise false if an exception should be thrown.</param>
        public void Launch(NSDictionary launchOptions, bool ignoreInvalidNotification = true)
        {
            this.Launch(null, iOSPushNotificationConverter.FromLaunchOptions(launchOptions), ignoreInvalidNotification);
        }

        /// <summary>
        /// Launches the initial view for the application asynchronously.
        /// </summary>
        /// <param name="authenticationProcess">An optional authentication process.</param>
        /// <param name="launchOptions">
        /// The launch options for an iOS application containing the data for a notification if the app was launched from the user responding to a notification alert.
        /// The dicationary will be ignored if it is empty or null.
        /// </param>
        /// <param name="ignoreInvalidNotification">True if an invalid notification should be ignored while launching, otherwise false if an exception should be thrown.</param>
        public void Launch(IAuthenticationProcess authenticationProcess, NSDictionary launchOptions, bool ignoreInvalidNotification = true)
        {
            this.Launch(authenticationProcess, iOSPushNotificationConverter.FromLaunchOptions(launchOptions), ignoreInvalidNotification);
        }

        /// <summary>
        /// Occurs when the current device has been successfully registered for remote notifications with the Apple Push Notification Server.
        /// </summary>
        /// <param name="notificationToken">A notification token uniquely identifying this device for sending push notifications.</param>
        public void HandleRegisteredForRemoteNotifications(string notificationToken)
        {
            // TODO: what if this fails?
            this.UpdatePushNotificationToken(notificationToken);
        }

        /// <summary>
        /// Handles when a push notification has been received.
        /// </summary>
        /// <param name="userInfo">The data received from the notification.</param>
        public void HandlePushNotificationReceived(NSDictionary userInfo)
        {
            this.HandlePushNotificationReceived(iOSPushNotificationConverter.FromRemoteNotification(userInfo));
        }

        /// <summary>
        /// Creates a navigation instanced used to navigate between screens in the application.
        /// </summary>
        /// <returns>A navigation instance.</returns>
        protected override INavigation CreateNavigation()
        {
            return new Navigation(new UIViewControllerFactory(this, this.viewControllerTypes));
        }

        /// <summary>
        /// Creates a push notification registry for the current application.
        /// </summary>
        /// <param name="notificationServerRegistration">An object responsible for registering the current device to receive push notifications from a remote server.</param>
        protected override PushNotificationRegistry CreatePushNotificationRegistry(IPushNotificationServerRegistration notificationServerRegistration)
        {
            return new iOSPushNotificationRegistry(notificationServerRegistration, this.SystemInformation);
        }

        /// <summary>
        /// Invokes the specified action on the main thread.
        /// </summary>
        /// <param name="action">An action to invoke.</param>
        protected override void InvokeOnMainThread(Action action)
        {
            this.appDelegate.InvokeOnMainThread(action);
        }

        private void Initialize()
        {
            if (this.viewControllerTypes == null)
            {
                this.viewControllerTypes = ViewControllerTypes.FromAssembly(this.appDelegate.GetType().GetTypeInfo().Assembly);
            }

            this.addressBook = new Lazy<iOSAddressBook>(() => new iOSAddressBook());
            this.alert = new Lazy<iOSApplicationAlert>(() => new iOSApplicationAlert(this.GetPresentingViewController));
            this.systemInformation = new Lazy<iOSSystemInformation>(() => new iOSSystemInformation());
            this.networkStatus = new Lazy<iOSNetworkStatus>(() => new iOSNetworkStatus());
            this.componentCatalog = new Lazy<iOSComponentCatalog>(() => new iOSComponentCatalog(this.GetPresentingViewController));
            this.deviceCatalog = new Lazy<iOSDeviceCatalog>(() => new iOSDeviceCatalog());
            this.deviceSettings = new Lazy<iOSDeviceSettings>(() => new iOSDeviceSettings());
            this.statusBar = new Lazy<iOSDeviceStatusBar>(() => new iOSDeviceStatusBar());
        }

        private UIViewController GetPresentingViewController()
        {
            return 
                this.appDelegate.Window.RootViewController.PresentedViewController ?? 
                this.appDelegate.Window.RootViewController;
        }
    }
}