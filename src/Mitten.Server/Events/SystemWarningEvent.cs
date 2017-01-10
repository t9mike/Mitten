namespace Mitten.Server.Events
{
    /// <summary>
    /// Represents a warning about the current health or state of the system.
    /// </summary>
    public sealed class SystemWarningEvent : SystemEvent
    {
        /// <summary>
        /// Initializes a new instance of the SystemWarningEvent class.
        /// </summary>
        /// <param name="description">A description of the warning.</param>
        public SystemWarningEvent(string description)
        {
            Throw.IfArgumentNullOrWhitespace(description, nameof(description));
            this.Description = description;
        }

        /// <summary>
        /// Gets a description of the warning.
        /// </summary>
        public string Description { get; }
    }
}