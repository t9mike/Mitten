namespace Mitten.Server.Commands
{
    /// <summary>
    /// The event that is raised when a command has started execution.
    /// </summary>
    public class CommandExecutionStartedEvent : CommandEvent
    {
        /// <summary>
        /// Initializes a new instance of the CommandExecutionStartedEvent class.
        /// </summary>
        /// <param name="commandKey">The key for the command.</param>
        internal CommandExecutionStartedEvent(CommandKey commandKey)
            : base(commandKey)
        {
        }
    }
}
