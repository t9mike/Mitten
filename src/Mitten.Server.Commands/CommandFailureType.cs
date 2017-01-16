namespace Mitten.Server.Commands
{
    /// <summary>
    /// Defines the failure types for a command.
    /// </summary>
    public enum CommandFailureType
    {
        /// <summary>
        /// Indicates that a command failed due to a timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// Indicates that an exception was thrown from within a command and was not caused by client error.
        /// </summary>
        CommandException,
    }
}