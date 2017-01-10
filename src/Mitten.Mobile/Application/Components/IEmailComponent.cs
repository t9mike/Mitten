namespace Mitten.Mobile.Application.Components
{
    /// <summary>
    /// Defines an component that provides access to a device's email client.
    /// </summary>
    public interface IEmailComponent
    {
        /// <summary>
        /// Determines whether or not the current device supports composing and sending emails.
        /// </summary>
        /// <returns>True if the device supports sending emails.</returns>
        bool CanComposeEmail();

        /// <summary>
        /// Composes a new email to be sent.
        /// </summary>
        /// <param name="recipient">The receipient to send to.</param>
        /// <param name="subject">The subject for the email.</param>
        void ComposesEmail(string recipient, string subject);
    }
}