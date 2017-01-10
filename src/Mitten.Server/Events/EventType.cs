namespace Mitten.Server.Events
{
    /// <summary>
    /// Defines a type of event.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Identifies an event that broadcasts information about the business concepts/facts in the application.
        /// </summary>
        Application,

        /// <summary>
        /// Identifies an event broadcasts technical details about the state of the system.
        /// </summary>
        System,
    }
}