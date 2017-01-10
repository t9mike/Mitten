using System;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Represents an option for an alert.
    /// </summary>
    public class AlertOption
    {
        private AlertOption(AlertOptionType alertOptionType, string text, Action callback)
        {
            Throw.IfArgumentNullOrWhitespace(text, nameof(text));
            Throw.IfArgumentNull(callback, nameof(callback));

            this.AlertOptionType = alertOptionType;
            this.Text = text;
            this.Callback = callback;
        }

        /// <summary>
        /// Gets the type of alert option.
        /// </summary>
        public AlertOptionType AlertOptionType { get; private set; }

        /// <summary>
        /// Gets the text for the alert.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a callback to be invoked if the option was chosen by the user.
        /// </summary>
        public Action Callback { get; private set; }

        /// <summary>
        /// Creates a new standard alert option.
        /// </summary>
        /// <param name="text">The text for the alert.</param>
        /// <param name="callback">A callback to be invoked if the option was chosen by the user.</param>
        /// <returns>A new alert option.</returns>
        public static AlertOption CreateStandardOption(string text, Action callback)
        {
            return new AlertOption(AlertOptionType.Standard, text, callback);
        }

        /// <summary>
        /// Creates a new destructive alert option.
        /// </summary>
        /// <param name="text">The text for the alert.</param>
        /// <param name="callback">A callback to be invoked if the option was chosen by the user.</param>
        /// <returns>A new alert option.</returns>
        public static AlertOption CreateDestructiveOption(string text, Action callback)
        {
            return new AlertOption(AlertOptionType.Destructive, text, callback);
        }

        /// <summary>
        /// Creates a new cancel alert option.
        /// </summary>
        /// <param name="text">The text for the alert.</param>
        /// <param name="callback">A callback to be invoked if the option was chosen by the user.</param>
        /// <returns>A new alert option.</returns>
        public static AlertOption CreateCancelOption(string text, Action callback)
        {
            return new AlertOption(AlertOptionType.Cancel, text, callback);
        }
    }
}