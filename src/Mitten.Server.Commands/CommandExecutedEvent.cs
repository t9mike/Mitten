namespace Mitten.Server.Commands
{
    /// <summary>
    /// The event that is raised when a command has finished executing.
    /// </summary>
    public class CommandExecutedEvent : CommandEvent
    {
        /// <summary>
        /// Initializes a new instance of the CommandDoneEvent class.
        /// </summary>
        /// <param name="commandKey">The key for the command.</param>
        /// <param name="commandResult">The result of the command.</param>
        internal CommandExecutedEvent(CommandKey commandKey, CommandResult commandResult)
            : base(commandKey)
        {
            Throw.IfArgumentNull(commandResult, nameof(commandResult));
            this.CommandResult = commandResult;
        }

        /// <summary>
        /// Gets the result of the command.
        /// </summary>
        public CommandResult CommandResult { get; private set; }
    }
}
