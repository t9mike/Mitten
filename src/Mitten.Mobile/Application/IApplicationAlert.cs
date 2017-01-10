using System;
using System.Collections.Generic;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Defines an object that can provide an alert to the user regardless of the screen they are on.
    /// </summary>
    public interface IApplicationAlert
    {
        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        void ShowAlert(string message);

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        void ShowAlert(string message, string title);

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="buttonText">Optional text for the button showed with the alert.</param>
        void ShowAlert(string message, string title, string buttonText);

        /// <summary>
        /// Shows an alert to the user with the specified message.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="buttonText">Optional text for the button showed with the alert.</param>
        /// <param name="closed">Gets invoked when the alert has been closed.</param>
        void ShowAlert(string message, string title, string buttonText, Action closed);

        /// <summary>
        /// Shows an alert to the user composed of a set of options for the user to choose from.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="options">A set of options for the alert.</param>
        /// <param name="alertStyle">Identifies the style for the alert.</param>
        void ShowAlertWithOptions(
            string message, 
            string title, 
            IEnumerable<AlertOption> options, 
            AlertStyle alertStyle);

        /// <summary>
        /// Shows an alert to the user asking for confirmation to a user action.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">An optional title for the alert.</param>
        /// <param name="trueButtonText">The text for the button that signals true for the confirmation.</param>
        /// <param name="falseButtonText">The text for the button that signals false for the confirmation.</param>
        /// <param name="confirm">A method that signals whether or not the user confirms the action.</param>
        void ShowConfirmationAlert(
            string message,
            string title,
            string trueButtonText,
            string falseButtonText,
            Action<bool> confirm);
    }
}