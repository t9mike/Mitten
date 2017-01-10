using System;
using System.Collections.Generic;
using System.Linq;
using Mitten.Mobile.Application;
using UIKit;

namespace Mitten.Mobile.iOS.Application
{
    /// <summary>
    /// Handles showing an alert to the user. 
    /// </summary>
    public class iOSApplicationAlert : IApplicationAlert
    {
        private readonly Func<UIViewController> getPresentingController;

        /// <summary>
        /// Initializes a new instance of the <see cref="iOSApplicationAlert"/> class.
        /// </summary>
        /// <param name="getPresentingController">Gets the view controller responsible for presenting the alert.</param>
        public iOSApplicationAlert(Func<UIViewController> getPresentingController)
        {
            Throw.IfArgumentNull(getPresentingController, "getPresentingController");
            this.getPresentingController = getPresentingController;
        }

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        public void ShowAlert(string message)
        {
            this.ShowAlert(message, string.Empty);
        }

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        public void ShowAlert(string message, string title)
        {
            this.ShowAlert(message, title, "OK");
        }

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="buttonText">Optional text for the button showed with the alert.</param>
        public void ShowAlert(string message, string title, string buttonText)
        {
            this.ShowAlert(message, title, buttonText, null);
        }

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="buttonText">Optional text for the button showed with the alert.</param>
        /// <param name="closed">Gets invoked when the alert has been closed.</param>
        public void ShowAlert(string message, string title, string buttonText, Action closed)
        {
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            alert.AddAction(
                UIAlertAction.Create(
                    buttonText,
                    UIAlertActionStyle.Default,
                    _ =>
                    {
                        if (closed != null)
                        {
                            closed();
                        }
                    }));

            this.getPresentingController().PresentViewController(alert, true, null);
        }

        /// <summary>
        /// Shows an alert to the user composed of a set of options for the user to choose from.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="options">A set of options for the alert.</param>
        /// <param name="alertStyle">Identifies the style for the alert.</param>
        public void ShowAlertWithOptions(
            string message,
            string title,
            IEnumerable<AlertOption> options,
            AlertStyle alertStyle)
        {
            if (options == null || options.Count() < 2)
            {
                throw new ArgumentException("At least 2 options must be specified.", nameof(options));
            }

            UIAlertControllerStyle alertControllerStyle;
            if (alertStyle == AlertStyle.Dialog)
            {
                alertControllerStyle = UIAlertControllerStyle.Alert;
            }
            else if (alertStyle == AlertStyle.ActionSheet)
            {
                alertControllerStyle = UIAlertControllerStyle.ActionSheet;
            }
            else
            {
                throw new ArgumentException("Unexpected alert style (" + alertStyle + ").", nameof(alertStyle));
            }

            UIAlertController alert = UIAlertController.Create(title, message, alertControllerStyle);

            foreach (AlertOption option in options)
            {
                UIAlertActionStyle style;

                if (option.AlertOptionType == AlertOptionType.Standard)
                {
                    style = UIAlertActionStyle.Default;
                }
                else if (option.AlertOptionType == AlertOptionType.Destructive)
                {
                    style = UIAlertActionStyle.Destructive;
                }
                else if (option.AlertOptionType == AlertOptionType.Cancel)
                {
                    style = UIAlertActionStyle.Cancel;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected AlertOptionType (" + option.AlertOptionType + ").");
                }

                alert.AddAction(UIAlertAction.Create(option.Text, style, _ => option.Callback()));
            }

            this.getPresentingController().PresentViewController(alert, true, null);
        }

        /// <summary>
        /// Shows an alert to the user asking for confirmation to a user action.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="trueButtonText">The text for the button that signals true for the confirmation.</param>
        /// <param name="falseButtonText">The text for the button that signals false for the confirmation.</param>
        /// <param name="confirm">A method that signals whether or not the user confirms the action.</param>
        public void ShowConfirmationAlert(
            string message,
            string title,
            string trueButtonText,
            string falseButtonText,
            Action<bool> confirm)
        {
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            alert.AddAction(UIAlertAction.Create(falseButtonText, UIAlertActionStyle.Default, _ => confirm(false)));
            alert.AddAction(UIAlertAction.Create(trueButtonText, UIAlertActionStyle.Default, _ => confirm(true)));

            this.getPresentingController().PresentViewController(alert, true, null);
        }
    }
}