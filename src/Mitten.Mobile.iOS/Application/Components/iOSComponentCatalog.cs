using System;
using Mitten.Mobile.Application.Components;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    public class iOSComponentCatalog : IComponentCatalog
    {
        private readonly Lazy<IBrowserComponent> browser;
        private readonly Lazy<ICameraComponent> camera;
        private readonly Lazy<IEmailComponent> email;
        private readonly Lazy<IMediaLibrary> mediaLibrary;
        private readonly Lazy<IPhoneComponent> phone;

        /// <summary>
        /// Initializes a new instance of the iOSComponentCatalog class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting the alert.</param>
        internal iOSComponentCatalog(Func<UIViewController> getPresentingController)
        {
            Throw.IfArgumentNull(getPresentingController, "getPresentingController");

            this.browser = new Lazy<IBrowserComponent>(() => new iOSBrowserComponent());
            this.camera = new Lazy<ICameraComponent>(() => new iOSCameraComponent(getPresentingController));
            this.email = new Lazy<IEmailComponent>(() => new iOSEmailComponent(getPresentingController));
            this.mediaLibrary = new Lazy<IMediaLibrary>(() => new iOSMediaLibrary(getPresentingController));
            this.phone = new Lazy<IPhoneComponent>(() => new iOSPhoneComponent());
        }

        /// <summary>
        /// Gets a component that provides access to the web browser.
        /// </summary>
        public IBrowserComponent Browser => this.browser.Value;

        /// <summary>
        /// Gets a component that provides high-level access to the camera.
        /// </summary>
        public ICameraComponent Camera => this.camera.Value;

        /// <summary>
        /// Gets a component that provides access to the default email client on the device.
        /// </summary>
        public IEmailComponent Email => this.email.Value;

        /// <summary>
        /// Gets a component that provides access to the media library.
        /// </summary>
        public IMediaLibrary MediaLibrary => this.mediaLibrary.Value;

        /// <summary>
        /// Gets a component that provides access to the phone.
        /// </summary>
        public IPhoneComponent Phone => this.phone.Value;
    }
}