using System;
using System.Threading.Tasks;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Wraps a function so it can be executed as a an async command.
    /// </summary>
    internal class AsyncDelegateCommand<TResponse> : AsyncCommand<TResponse>
    {
        private readonly Func<Task<TResponse>> executeCommand;

        /// <summary>
        /// Initializes a new instance of the AsyncDelegateCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        public AsyncDelegateCommand(string groupName, string commandName, CommandProperties commandProperties, Func<Task<TResponse>> executeCommand)
            : base(groupName, commandName, commandProperties)
        {
            Throw.IfArgumentNull(executeCommand, nameof(executeCommand));
            this.executeCommand = executeCommand;
        }

        /// <summary>
        /// Runs the command logic asynchronously and returns a Task.
        /// </summary>
        /// <returns>The Task from the command.</returns>
        protected override Task<TResponse> RunAsync()
        {
            return this.executeCommand();
        }
    }
}
