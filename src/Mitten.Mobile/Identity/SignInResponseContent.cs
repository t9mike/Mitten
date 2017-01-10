namespace Mitten.Mobile.Identity
{
    /// <summary>
    /// Represents the response content for a service sign-in request.
    /// </summary>
    public class SignInResponseContent
    {
        /// <summary>
        /// Initializes a new instance of the SignInResponseContent class.
        /// </summary>
        /// <param name="account">The account for the person that signed in.</param>
        /// <param name="token">The authentication token.</param>
        public SignInResponseContent(IAccount account, string token)
        {
            Throw.IfArgumentNull(account, nameof(account));
            Throw.IfArgumentNullOrWhitespace(token, nameof(token));

            this.Account = account;
            this.Token = token;
        }

        /// <summary>
        /// Gets the account for the person that signed in.
        /// </summary>
        public IAccount Account { get; private set; }

        /// <summary>
        /// Gets an authentication token.
        /// </summary>
        public string Token { get; private set; }
    }
}