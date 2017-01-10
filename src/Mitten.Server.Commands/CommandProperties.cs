namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents a set of properties defining various execution behaviors for a command.
    /// </summary>
    public class CommandProperties
    {
        /// <summary>
        /// The default properties for command execution.
        /// </summary>
        public static readonly CommandProperties Default = new CommandProperties(5000);

        /// <summary>
        /// Initializes a new instance of the CommandProperties class.
        /// </summary>
        /// <param name="executionTimeoutMilliseconds">The timeout, in milliseconds, for the execution of the command.</param>
        public CommandProperties(int executionTimeoutMilliseconds)
        {
            this.ExecutionTimeoutMilliseconds = executionTimeoutMilliseconds;
        }

        /// <summary>
        /// Gets the timeout, in milliseconds, for the execution of the command.
        /// </summary>
        public int ExecutionTimeoutMilliseconds { get; private set; }

        /// <summary>
        /// Gets updated CommandProperties with the specified execution timeout.
        /// </summary>
        /// <param name="executionTimeoutMilliseconds">The timeout, in milliseconds, for the execution of the command.</param>
        /// <returns>A new CommandProperties instance.</returns>
        public CommandProperties WithExecutionTimeoutMilliseconds(int executionTimeoutMilliseconds)
        {
            return new CommandProperties(executionTimeoutMilliseconds);
        }
    }
}
