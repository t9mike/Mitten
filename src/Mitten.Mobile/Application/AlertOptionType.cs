namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Defines the type for an alert option.
    /// </summary>
    public enum AlertOptionType
    {
        /// <summary>
        /// Identifies the option as a standard choice from an option set.
        /// </summary>
        Standard,

        /// <summary>
        /// Identifies that the option represents a destructive action, such as a deletion.
        /// </summary>
        Destructive,

        /// <summary>
        /// Identifies that the option represents a cancellation action.
        /// </summary>
        Cancel,
    }
}