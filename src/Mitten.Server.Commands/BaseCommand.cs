using System;
using System.Threading;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Base class for commands.
    /// </summary>
    /// <typeparam name="TResponse">The Type of response returned from the command.</typeparam>
    public abstract class BaseCommand<TResponse>
    {
        private static class Constants
        {
            public const int FalseInt = 0;
            public const int TrueInt = 1;
        }

        private CommandExecutionContext executionContext;
        private int hasBeenAcquired;

        /// <summary>
        /// Initializes a new instance of the BaseCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        internal BaseCommand(string groupName, string commandName, CommandProperties commandProperties)
        {
            Throw.IfArgumentNullOrWhitespace(groupName, "groupName");
            Throw.IfArgumentNull(commandProperties, "commandProperties");

            this.GroupName = groupName;
            this.CommandName =
                string.IsNullOrWhiteSpace(commandName)
                ? BaseCommand<TResponse>.GetCommandName(this.GetType())
                : commandName;

            this.CommandProperties = commandProperties;
            this.CommandKey = new CommandKey(this.GroupName, this.CommandName);
        }

        /// <summary>
        /// Gets the group name for the command.
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the properties for the command.
        /// </summary>
        internal CommandProperties CommandProperties { get; private set; }

        /// <summary>
        /// Gets the key for this command.
        /// </summary>
        internal CommandKey CommandKey { get; private set; }

        /// <summary>
        /// Raises a warning for the current command.
        /// </summary>
        /// <param name="message">A warning message.</param>
        protected void RaiseWarning(string message)
        {
            this.executionContext.RaiseWarning(this, message);
        }

        /// <summary>
        /// Gets an observable to be executed for the current command.
        /// </summary>
        /// <typeparam name="TResponse">The Type of response from the command.</typeparam>
        /// <returns>An observable.</returns>
        internal protected abstract IObservable<TResponse> GetExecutionObservable();

        /// <summary>
        /// Attempts to acquire execution permission for this command.
        /// </summary>
        /// <param name="executionContext">The context in which the command will be executed.</param>
        /// <returns>True if the command has been acquired for execution; otherwise false indicating that someone else has acquired the rights to execute this command.</returns>
        internal bool AcquireForExecution(CommandExecutionContext executionContext)
        {
            if (Interlocked.CompareExchange(ref this.hasBeenAcquired, Constants.TrueInt, Constants.FalseInt) == Constants.FalseInt)
            {
                this.executionContext = executionContext;
                return true;
            }

            return false;
        }

        private static string GetCommandName(Type type)
        {
            return type.Name;
        }
    }
}
