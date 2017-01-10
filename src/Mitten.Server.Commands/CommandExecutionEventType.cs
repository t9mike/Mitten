namespace Mitten.Server.Commands
{
    /// <summary>
    /// Defines the types of events emitted by the execution of a command.
    /// </summary>
    public enum CommandExecutionEventType
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Indicates that a command executed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates that the command failed due to a bad request.
        /// </summary>
        BadRequest,

        /// <summary>
        /// Indicates that an exception occurred from a running command.
        /// </summary>
        CommandException,

        /// <summary>
        /// Indicates that the execution of the command failed due to a timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// Indicates that an unexpected internal failure occurred during the actual scheduling and execution of a command from within an execution context.
        /// </summary>
        InternalFailure,
    }
}
