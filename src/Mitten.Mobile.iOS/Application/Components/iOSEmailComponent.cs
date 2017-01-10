using System;
using Mitten.Mobile.Application.Components;
using MessageUI;
using UIKit;

namespace Mitten.Mobile.iOS.Application.Components
{
    /// <summary>
    /// Provides access to the email client installed on an iOS device.
    /// </summary>
    public class iOSEmailComponent : IEmailComponent
    {
        private readonly Func<UIViewController> getPresentingController;

        /// <summary>
        /// Initializes a new instance of the iOSEmailComponent class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting the email component.</param>
        public iOSEmailComponent(Func<UIViewController> getPresentingController)
        {
            Throw.IfArgumentNull(getPresentingController, nameof(getPresentingController));
            this.getPresentingController = getPresentingController;
        }

        /// <summary>
        /// Determines whether or not the current device supports composing and sending emails.
        /// </summary>
        /// <returns>True if the device supports sending emails.</returns>
        public bool CanComposeEmail()
        {
            return MFMailComposeViewController.CanSendMail;
        }

        /// <summary>
        /// Composes a new email to be sent.
        /// </summary>
        /// <param name="recipient">The receipient to send to.</param>
        /// <param name="subject">The subject for the email.</param>
        public void ComposesEmail(string recipient, string subject)
        {
            UIViewController presentingController = this.getPresentingController();
            MFMailComposeViewController mailController = new MFMailComposeViewController();

            mailController.SetToRecipients(new[] { recipient });
            mailController.SetSubject(subject);

            mailController.Finished +=
                (sender, e) => presentingController.BeginInvokeOnMainThread(
                    () => e.Controller.DismissViewController(true, null));

            presentingController.PresentViewController(mailController, true, null);
        }
    }
}
