using Mitten.Server.Events;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Base class for a command event.
    /// </summary>
    public abstract class CommandEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of the CommandEvent class.
        /// </summary>
        /// <param name="commandKey">The key for the command.</param>
        internal CommandEvent(CommandKey commandKey)
        {
            Throw.IfArgumentNull(commandKey, "commandKey");
            this.CommandKey = commandKey;
        }

        /// <summary>
        /// Gets the key for the command the event represents.
        /// </summary>
        public CommandKey CommandKey { get; private set; }
    }
}
