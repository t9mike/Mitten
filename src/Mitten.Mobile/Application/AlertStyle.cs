namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Defines the types of styles available for an alert.
    /// </summary>
    public enum AlertStyle
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Represents a modal alert dialog.
        /// </summary>
        Dialog,

        /// <summary>
        /// Represents an alert intended to present options/actions for the user as opposed to an alert message.
        /// </summary>
        ActionSheet,
    }
}