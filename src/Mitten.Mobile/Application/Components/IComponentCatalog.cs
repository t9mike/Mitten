namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Provides access to various integrated software components available on the device's platform.
    /// </summary>
    /// <remarks>
    /// These are UX components provided by the platform and may navigate the user out of the current application.
    /// </remarks>
    public interface IComponentCatalog
    {
        /// <summary>
        /// Gets a component that provides access to the web browser.
        /// </summary>
        IBrowserComponent Browser { get; }

        /// <summary>
        /// Gets a component that provides high-level access to the camera.
        /// </summary>
        ICameraComponent Camera { get; }

        /// <summary>
        /// Gets a component that provides access to the default email client on the device.
        /// </summary>
        IEmailComponent Email { get; }

        /// <summary>
        /// Gets a component that provides access to the media library.
        /// </summary>
        IMediaLibrary MediaLibrary { get; }

        /// <summary>
        /// Gets a component that provides access to the phone.
        /// </summary>
        IPhoneComponent Phone { get; }
    }
}