using Mitten.Mobile.Identity;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Contains information for the currntly logged in user session.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Represents an application session for an anonymous user.
        /// </summary>
        internal static readonly Session Anonymous = new Session();

        /// <summary>
        /// Initializes a new instance of the Session class.
        /// </summary>
        /// <param name="account">The accont for the currently signed in user.</param>
        /// <param name="securityToken">The security token for the current session used to communicate with the server.</param>
        public Session(IAccount account, string securityToken)
        {
            Throw.IfArgumentNull(account, nameof(account));
            Throw.IfArgumentNullOrWhitespace(securityToken, nameof(securityToken));

            this.Account = account;
            this.SecurityToken = securityToken;
        }

        private Session()
        {
            this.IsAnonymous = true;
        }

        /// <summary>
        /// Gets whether or not the current sessions is for an anonymous user.
        /// </summary>
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// Gets the accont for the currently signed in user.
        /// </summary>
        public IAccount Account { get; private set; }

        /// <summary>
        /// Gets the security token for the current session used to communicate with the server.
        /// </summary>
        public string SecurityToken { get; private set; }
    }
}