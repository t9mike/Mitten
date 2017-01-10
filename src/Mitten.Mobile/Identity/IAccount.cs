using System;

namespace Mitten.Mobile.Identity
{
    /// <summary>
    /// Represents a user account.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Gets the id for the account.
        /// </summary>
        Guid AccountId { get; }
    }
}