using System;
using System.Threading.Tasks;
using Mitten.Mobile.Model;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Represents a table view source that supports pagination.
    /// </summary>
    public interface IPageableTableViewSource<TEntity> : ITableViewSource<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Gets whether or not more pages of items are available.
        /// </summary>
        bool HasMorePages { get; }

        /// <summary>
        /// Gets the number of items per page.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Loads the next page of items asynchronously.
        /// </summary>
        /// <param name="onSuccess">A callback that gets invoked if the load operation was successful.</param>
        /// <returns>The load task.</returns>
        Task LoadNextPageAsync(Action onSuccess);
    }
}
