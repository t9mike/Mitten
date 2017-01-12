namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents the key for a command.
    /// </summary>
    public sealed class CommandKey
    {
        private readonly string commandGroup;
        private readonly string commandName;

        /// <summary>
        /// Initializes a new instance of the CommandKey class.
        /// </summary>
        /// <param name="commandGroup">The group for the command.</param>
        /// <param name="commandName">The name of the command.</param>
        public CommandKey(string commandGroup, string commandName)
        {
            Throw.IfArgumentNullOrWhitespace(commandGroup, "commandGroup");
            Throw.IfArgumentNullOrWhitespace(commandName, "commandName");

            this.commandGroup = commandGroup;
            this.commandName = commandName;
        }
        
        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if they are considered equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            CommandKey key = obj as CommandKey;

            if (key == null)
            {
                return false;
            }

            return CommandKey.AreEqual(this, key);
        }

        /// <summary>
        /// Gets the hash code for the key.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (this.commandGroup + this.commandName).GetHashCode();
        }

        /// <summary>
        /// Compares the objects for equality.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns>True if they are equal.</returns>
        public static bool operator ==(CommandKey lhs, CommandKey rhs)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return CommandKey.AreEqual(lhs, rhs);
        }

        /// <summary>
        /// Compares the objects for equality.
        /// </summary>
        /// <param name="lhs">The left hand side.</param>
        /// <param name="rhs">The right hand side.</param>
        /// <returns>True if they are not equal, otherwise false.</returns>
        public static bool operator !=(CommandKey lhs, CommandKey rhs)
        {
            return !(lhs == rhs);
        }

        private static bool AreEqual(CommandKey lhs, CommandKey rhs)
        {
            return
                lhs.commandGroup == rhs.commandGroup &&
                lhs.commandName == rhs.commandName;
        }
    }
}
