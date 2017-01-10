namespace Mitten.Server.Commands
{
    /// <summary>
    /// An event indicating a warning had occurred during the execution of a command.
    /// </summary>
    public class CommandWarningEvent : CommandEvent
    {
        /// <summary>
        /// Initializes a new instance of the CommandWarningEvent class.
        /// </summary>
        /// <param name="commandKey">The key for the command raising the warning.</param>
        /// <param name="commandGroup">The group for the command raising the warning.</param>
        /// <param name="commandName">The name for the command raising the warning.</param>
        /// <param name="message">A warning message.</param>
        internal CommandWarningEvent(CommandKey commandKey, string commandGroup, string commandName, string message)
            : base(commandKey)
        {
            Throw.IfArgumentNullOrWhitespace(commandGroup, "commandGroup");
            Throw.IfArgumentNullOrWhitespace(commandName, "commandName");
            Throw.IfArgumentNullOrWhitespace(message, "message");

            this.Message = message;
            this.CommandGroup = commandGroup;
            this.CommandName = commandName;
        }

        /// <summary>
        /// Gets the group for the command raising the warning.
        /// </summary>
        public string CommandGroup { get; private set; }

        /// <summary>
        /// Gets the name for the command raising the warning.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the message for the warning.
        /// </summary>
        public string Message { get; private set; }
    }
}