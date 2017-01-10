using System;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Wraps a function so it can be executed as a command.
    /// </summary>
    internal class DelegateCommand<TResponse> : Command<TResponse>
    {
        private readonly Func<TResponse> executeCommand;

        /// <summary>
        /// Initializes a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        public DelegateCommand(string groupName, string commandName, CommandProperties commandProperties, Func<TResponse> executeCommand)
            : base(groupName, commandName, commandProperties)
        {
            Throw.IfArgumentNull(executeCommand, nameof(executeCommand));
            this.executeCommand = executeCommand;
        }

        /// <summary>
        /// Runs the command logic and returns a response.
        /// </summary>
        /// <returns>The response from the command.</returns>
        protected override TResponse Run()
        {
            return this.executeCommand();
        }
    }
}