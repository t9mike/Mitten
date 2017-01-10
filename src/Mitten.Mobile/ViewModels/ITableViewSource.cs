using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mitten.Mobile.Model;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Defines a source that provides a list of entities for a table view.
    /// </summary>
    public interface ITableViewSource<TEntity> 
        where TEntity : Entity
    {
        /// <summary>
        /// Gets a list of entities for the table.
        /// </summary>
        IEnumerable<TEntity> Items { get; }

        /// <summary>
        /// Refreshes the list of items for the table asynchronously.
        /// </summary>
        /// <param name="onSuccess">A callback that gets invoked if the refresh operation was successful.</param>
        /// <returns>The refresh task.</returns>
        Task RefreshItemsAsync(Action onSuccess);
    }
}