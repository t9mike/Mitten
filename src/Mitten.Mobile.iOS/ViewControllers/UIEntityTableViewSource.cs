using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.Model;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// A table view source for the UIEntityTableViewController.
    /// </summary>
    internal class UIEntityTableViewSource<TEntity> : UITableViewSource
        where TEntity : Entity
    {
        private static class Constants
        {
            public const float EstimatedHeightForLoadingIndicator = 44.0f;
        }

        private readonly UITableView tableView;
        private readonly Func<TEntity, nfloat> estimateCellHeight;
        private readonly Func<TEntity, UIEntityTableViewCell<TEntity>> createTableCell;
        private readonly Func<UITableViewCell> createLoadingMorePagesCell;
        private readonly Action<UIEntityTableViewCell<TEntity>, NSIndexPath> requestImageForCell;
        private readonly Action<UIEntityTableViewCell<TEntity>> onRowSelected;
        private readonly Action<UIEntityTableViewCell<TEntity>> onRowDeselected;

        private TEntity[] items;
        private bool hasMorePages;

        /// <summary>
        /// Initializes a new instance of the UIEntityTableViewSource class.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="items">The initial list of items for the table view.</param>
        /// <param name="hasMorePages">Identifies whether or not more pages of items are available.</param>
        /// <param name="estimateCellHeight">Estimates the height of the cell for a specified entity.</param>
        /// <param name="createTableCell">A factory method for creating table view cells.</param>
        /// <param name="createLoadingMorePagesCell">A factory method for creating the table cell at the end of the list to display that more pages are being loaded.</param>
        /// <param name="requestImageForCell">Requests an image for the specified cell at the given index.</param>
        /// <param name="onRowSelected">A callback that will be invoked when a row has been selected.</param>
        /// <param name="onRowDeselected">A callback that will be invoked when a row has been deselected.</param>
        public UIEntityTableViewSource(
            UITableView tableView,
            IEnumerable<TEntity> items,
            bool hasMorePages,
            Func<TEntity, nfloat> estimateCellHeight,
            Func<TEntity, UIEntityTableViewCell<TEntity>> createTableCell,
            Func<UITableViewCell> createLoadingMorePagesCell,
            Action<UIEntityTableViewCell<TEntity>, NSIndexPath> requestImageForCell,
            Action<UIEntityTableViewCell<TEntity>> onRowSelected,
            Action<UIEntityTableViewCell<TEntity>> onRowDeselected)
        {
            this.tableView = tableView;
            this.items = items.ToArray();
            this.hasMorePages = hasMorePages;
            this.estimateCellHeight = estimateCellHeight;
            this.createTableCell = createTableCell;
            this.createLoadingMorePagesCell = createLoadingMorePagesCell;
            this.requestImageForCell = requestImageForCell;
            this.onRowSelected = onRowSelected;
            this.onRowDeselected = onRowDeselected;
        }

        /// <summary>
        /// Occurs when the cell at the specified index is about to be displayed.
        /// </summary>
        public event Action<NSIndexPath> CellWillBeDisplayed = delegate { };

        /// <summary>
        /// Gets whether or not the table view was last scrolled/dragged downward by the user.
        /// </summary>
        public bool WasScrolledDown { get; private set; }

        /// <summary>
        /// Gets the estimated height for the cell at the specified index.
        /// </summary>
        /// <param name="tableView">The table view the cell belongs to.</param>
        /// <param name="indexPath">The index path for the cell.</param>
        /// <returns>An estimated height.</returns>
        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            if (this.IsLoadingIndicator(indexPath))
            {
                return Constants.EstimatedHeightForLoadingIndicator;
            }

            return this.estimateCellHeight(this.items.ElementAt(indexPath.Row));
        }

        /// <summary>
        /// Occurs when the user is about to end dragging the scroll view for the table.
        /// </summary>
        /// <param name="scrollView">The scroll view.</param>
        /// <param name="velocity">The scroll velocity.</param>
        /// <param name="targetContentOffset">Target content offset.</param>
        public override void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
        {
            this.WasScrolledDown = velocity.Y < 0;
        }

        /// <summary>
        /// Occurs when a cell is about to be displayed.
        /// </summary>
        /// <param name="tableView">The table view that owns the cell.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="indexPath">The index path for the cell.</param>
        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            this.CellWillBeDisplayed(indexPath);
        }

        /// <summary>
        /// Gets the number of rows in the specified section.
        /// </summary>
        /// <param name="tableview">The table view for the section.</param>
        /// <param name="section">The section.</param>
        /// <returns>The number of rows in the section.</returns>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return
                this.hasMorePages
                ? this.items.Count() + 1
                : this.items.Count();
        }

        /// <summary>
        /// Gets the cell for the specified table at the given index.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        /// <returns>A table view cell.</returns>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (this.IsLoadingIndicator(indexPath))
            {
                return this.createLoadingMorePagesCell();
            }

            TEntity entity = this.items.ElementAt(indexPath.Row);
            UIEntityTableViewCell<TEntity> cell = this.createTableCell(entity);

            cell.Initialize(entity);
            if (cell.IsRemoteImageSupported)
            {
                this.requestImageForCell(cell, indexPath);
            }

            return cell;
        }

        /// <summary>
        /// Occurs when a row has been selected.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            this.onRowSelected((UIEntityTableViewCell<TEntity>)this.tableView.CellAt(indexPath));
        }

        /// <summary>
        /// Occurs when a row has been deselected.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            this.onRowDeselected((UIEntityTableViewCell<TEntity>)this.tableView.CellAt(indexPath));
        }

        /// <summary>
        /// Inserts a new page of items to the end of the current list of items.
        /// This is an optimized version of the UpdateItems method.
        /// </summary>
        /// <param name="modifiedList">The most current version of the list.</param>
        /// <param name="hasMorePages">A value indicating whether or not there are still more pages of activities available.</param>
        public void InsertNewPage(IEnumerable<TEntity> modifiedList, bool hasMorePages)
        {
            int totalNumberOfItems = this.items.Count();
            int numberOfItemsToInsert = modifiedList.Count() - totalNumberOfItems;

            if (numberOfItemsToInsert <= 0)
            {
                throw new InvalidOperationException("The updated list has the same number of items as the current list.");
            }

            if (this.hasMorePages != hasMorePages)
            {
                // If the has more pages flag changes we need to make sure this cell is
                // accounted for when adding new cells. For instance, if there are no more
                // pages then we do not want the activity indicator cell to be shown.

                this.hasMorePages = hasMorePages;
                if (this.hasMorePages)
                {
                    numberOfItemsToInsert++;
                }
                else
                {
                    this.tableView.DeleteRows(
                        new[] { NSIndexPath.FromRowSection(totalNumberOfItems, 0) },
                        UITableViewRowAnimation.None);
                }
            }

            List<NSIndexPath> newRows = new List<NSIndexPath>();

            for (int i = 0; i < numberOfItemsToInsert; i++)
            {
                newRows.Add(NSIndexPath.FromRowSection(totalNumberOfItems + i, 0));
            }

            this.items = modifiedList.ToArray();
            this.tableView.InsertRows(newRows.ToArray(), UITableViewRowAnimation.None);
        }

        /// <summary>
        /// Updates the list of items based on the most current list of items.
        /// </summary>
        /// <param name="modifiedList">Modified list.</param>
        public void UpdateItems(IEnumerable<TEntity> modifiedList)
        {
            IEnumerable<TEntity> originalList = this.items;
            this.items = modifiedList.ToArray();

            this.HandleDeletes(originalList, modifiedList);
            this.HandleUpdates(originalList, modifiedList);
            this.HandleInserts(originalList, modifiedList);
        }

        private void HandleDeletes(IEnumerable<TEntity> originalList, IEnumerable<TEntity> modifiedList)
        {
            List<NSIndexPath> deletedRows = new List<NSIndexPath>();
            for (int i = 0; i < originalList.Count(); i++)
            {
                TEntity item = originalList.ElementAt(i);
                if (!modifiedList.Contains(item))
                {
                    deletedRows.Add(NSIndexPath.FromRowSection(i, 0));
                }
            }

            if (deletedRows.Count > 0)
            {
                this.tableView.DeleteRows(deletedRows.ToArray(), UITableViewRowAnimation.Automatic);
            }
        }

        private void HandleUpdates(IEnumerable<TEntity> originalList, IEnumerable<TEntity> modifiedList)
        {
            List<NSIndexPath> modifiedRows = new List<NSIndexPath>();
            for (int i = 0; i < modifiedList.Count(); i++)
            {
                TEntity modifiedItem = modifiedList.ElementAt(i);
                TEntity originalItem = originalList.SingleOrDefault(item => item == modifiedItem);

                if (originalItem != default(TEntity) &&
                    originalItem.GetVersion() != modifiedItem.GetVersion())
                {
                    modifiedRows.Add(NSIndexPath.FromRowSection(i, 0));
                }
            }

            if (modifiedRows.Count > 0)
            {
                this.tableView.ReloadRows(modifiedRows.ToArray(), UITableViewRowAnimation.Automatic);
            }
        }

        private void HandleInserts(IEnumerable<TEntity> originalList, IEnumerable<TEntity> modifiedList)
        {
            List<NSIndexPath> newRows = new List<NSIndexPath>();

            for (int i = 0; i < modifiedList.Count(); i++)
            {
                TEntity newItem = modifiedList.ElementAt(i);
                if (!originalList.Any(item => item == newItem))
                {
                    newRows.Add(NSIndexPath.FromRowSection(i, 0));
                }
            }

            if (newRows.Count > 0)
            {
                this.tableView.InsertRows(newRows.ToArray(), UITableViewRowAnimation.Automatic);
            }
        }

        private bool IsLoadingIndicator(NSIndexPath indexPath)
        {
            return this.items.Length == indexPath.Row;
        }
    }
}
