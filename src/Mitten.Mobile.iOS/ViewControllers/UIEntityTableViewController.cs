using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.iOS.Graphics;
using Mitten.Mobile.iOS.Views;
using Mitten.Mobile.Model;
using Mitten.Mobile.Remote;
using Mitten.Mobile.Themes;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Handles providing a list of entities in a basic table.
    /// </summary>
    public abstract class UIEntityTableViewController<TViewModel, TEntity> : UIViewController<TViewModel>
        where TViewModel : ViewModel
        where TEntity : Entity
    {
        private static class Constants
        {
            public const string RefreshIndicatorText = "Refreshing...";
            public const int RefreshIndicatorHeight = 25;
            public const float RefreshIndicatorAnimationDuration = 0.2f;

            public const float DefaultOverlyaDelay = 0.2f;
            public const float AutoRefreshWhileShownOverlayDelay = 1.0f;

            public const float DefaultEstimatedCellHeight = 44.0f;
        }

        private RemoteImageCatalog imageCatalog;
        private UIEntityTableViewSource<TEntity> uiTableViewSource;
        private IPageableTableViewSource<TEntity> tableSource;

        private UIRefreshControl manualRefreshControl;
        private UIColor manualRefreshBackgroundColor;
        private UIColor manualRefreshTintColor;

        private UILabel refreshIndicator;
        private LabelTheme refreshIndicatorTheme;
        private string refreshIndicatorText;

        private bool allowManualRefresh;
        private bool isDisposed;
        private bool isLoadingNextPage;
        private bool isShown;
        private bool isRefreshNeeded;
        private bool isRefreshIndicatorShown;

        /// <summary>
        /// Initializes a new instance of the UITableViewController class.
        /// </summary>
        protected UIEntityTableViewController()
        {
            this.allowManualRefresh = true;
            this.refreshIndicatorText = Constants.RefreshIndicatorText;
            this.ClearsSelectionOnViewWillAppear = true;
        }

        /// <summary>
        /// Initializes a new instance of the UITableViewController class.
        /// </summary>
        /// <param name="handle">Object handle used by the runtime.</param>
        protected UIEntityTableViewController(IntPtr handle) 
            : base(handle)
        {
            this.allowManualRefresh = true;
            this.refreshIndicatorText = Constants.RefreshIndicatorText;
            this.ClearsSelectionOnViewWillAppear = true;
        }

        /// <summary>
        /// Gets or sets whether or not the user can maually refresh the table by pulling down.
        /// </summary>
        public bool AllowManualRefresh
        {
            get { return this.allowManualRefresh; }
            set
            {
                if (this.allowManualRefresh != value)
                {
                    this.allowManualRefresh = value;
                    this.UpdateManualRefreshControlState();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text for the refresh indicator.
        /// </summary>
        public string RefreshIndicatorText
        {
            get { return this.refreshIndicatorText; }
            set
            {
                Throw.IfArgumentNullOrWhitespace(value, nameof(value));

                if (this.refreshIndicatorText != value)
                {
                    this.refreshIndicatorText = value;
                    if (this.refreshIndicator != null)
                    {
                        this.refreshIndicator.Text = this.refreshIndicatorText;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for the manual refresh control.
        /// </summary>
        public UIColor ManualRefreshBackgroundColor
        {
            get { return this.manualRefreshBackgroundColor; }
            set 
            {
                Throw.IfArgumentNull(value, nameof(value));

                if (this.manualRefreshBackgroundColor != value)
                {
                    this.manualRefreshBackgroundColor = value;
                    if (this.manualRefreshControl != null)
                    {
                        this.manualRefreshControl.BackgroundColor = this.manualRefreshBackgroundColor;
                    }
                } 
            }
        }

        /// <summary>
        /// Gets or sets the tint color for the manual refresh control.
        /// </summary>
        public UIColor ManualRefreshTintColor
        {
            get { return this.manualRefreshTintColor; }
            set
            {
                Throw.IfArgumentNull(value, nameof(value));

                if (this.manualRefreshTintColor != value)
                {
                    this.manualRefreshTintColor = value;
                    if (this.manualRefreshControl != null)
                    {
                        this.manualRefreshControl.TintColor = this.manualRefreshTintColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not all selected items will be cleared/deselected when the view appears.
        /// </summary>
        public bool ClearsSelectionOnViewWillAppear { get; set; }

        /// <summary>
        /// Gets the table view for this controller.
        /// </summary>
        protected abstract UITableView TableView { get; }

        /// <summary>
        /// Notifies the view controller that its view is about to be added to a view hierarchy.
        /// </summary>
        /// <param name="animated">If true, the appearance of the view is being animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (this.refreshIndicator != null)
            {
                this.refreshIndicator.Hidden = false;
            }

            if (this.TableView != null && this.TableView.IndexPathsForSelectedRows != null)
            {
                foreach (NSIndexPath indexPath in this.TableView.IndexPathsForSelectedRows)
                {
                    this.TableView.DeselectRow(indexPath, true);
                }
            }
        }

        /// <summary>
        /// Occurs when the view has appeared on screen to the user. If the data in this view is stale, we will refresh at this time.
        /// </summary>
        /// <param name="animated">True to indicate the transition was animated.</param>
        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (this.isRefreshNeeded)
            {
                this.isRefreshNeeded = false;
                await this.StartTableRefresh();
            }

            this.ResetRefreshIndicator();
            this.isShown = true;
        }

        /// <summary>
        /// Occurs when the view is about to be removed from the view hierarchy.
        /// </summary>
        /// <param name="animated">If true, the disappearance of the view is being animated.</param>
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (this.refreshIndicator != null)
            {
                this.refreshIndicator.Hidden = true;
            }
        }

        /// <summary>
        /// Occurs when the view has disappeared from the user's view.
        /// </summary>
        /// <param name="animated">True to indicate the transition was animated.</param>
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            this.isShown = false;
        }

        /// <summary>
        /// Occurs when the app is getting low on memory.
        /// </summary>
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (this.imageCatalog != null)
            {
                this.imageCatalog.ClearCache();
            }
        }

        /// <summary>
        /// Applies a label theme to the refresh indicator.
        /// </summary>
        /// <param name="theme">A label theme.</param>
        public void ApplyRefreshIndicatorTheme(LabelTheme theme)
        {
            Throw.IfArgumentNull(theme, nameof(theme));

            this.refreshIndicatorTheme = theme;
            if (this.refreshIndicator != null)
            {
                this.refreshIndicator.ApplyTheme(theme);
            }
        }

        /// <summary>
        /// Disposes the current instance.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected override void Dispose(bool disposing)
        {
            this.isDisposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes and updates any visual appearances for the views owned by this controller.
        /// </summary>
        protected override void InitializeViews()
        {
            this.InitializeManualRefreshControl();
            this.InitializeRefreshIndicator();

            this.View.AddSubview(this.refreshIndicator);
            this.View.BringSubviewToFront(this.refreshIndicator);

            base.InitializeViews();
        }

        /// <summary>
        /// Creates a new cell for the specified entity.
        /// </summary>
        /// <param name="entity">The entity associated with the cell.</param>
        /// <returns>A new table cell.</returns>
        protected abstract UIEntityTableViewCell<TEntity> CreateTableCell(TEntity entity);

        /// <summary>
        /// Initializes and binds a list of items to the table source.
        /// </summary>
        /// <param name="tableSource">A table source providing the list of items to display in the table.</param>
        /// <param name="imageCatalog">An optional remote image catalog to use if this table supports remote image downloading.</param>
        protected void BindTableSource(ITableViewSource<TEntity> tableSource, RemoteImageCatalog imageCatalog = null)
        {
            Throw.IfArgumentNull(tableSource, nameof(tableSource));
            this.BindTableSource(new TableViewSourceWithoutPages(tableSource), imageCatalog);
        }

        /// <summary>
        /// Initializes and binds a list of items to the table source.
        /// </summary>
        /// <param name="tableSource">A table source providing the list of items to display in the table.</param>
        /// <param name="imageCatalog">An optional remote image catalog to use if this table supports remote image downloading.</param>
        protected void BindTableSource(IPageableTableViewSource<TEntity> tableSource, RemoteImageCatalog imageCatalog = null)
        {
            Throw.IfArgumentNull(tableSource, nameof(tableSource));

            if (this.tableSource != null)
            {
                throw new InvalidOperationException("A table source has already been bound.");
            }

            this.tableSource = tableSource;
            this.uiTableViewSource =
                new UIEntityTableViewSource<TEntity>(
                    this.TableView,
                    this.tableSource.Items,
                    this.tableSource.HasMorePages,
                    this.EstimateHeightForCell,
                    this.CreateTableCell,
                    this.CreateLoadingMorePagesCell,
                    this.RequestImageForCell,
                    this.OnRowSelected,
                    this.OnRowDeselected);

            this.uiTableViewSource.CellWillBeDisplayed += this.StartLoadingNextPageIfNeeded;
            this.TableView.Source = this.uiTableViewSource;
            this.imageCatalog = imageCatalog;

            this.UpdateTableState();
        }

        /// <summary>
        /// Estimates the height for a cell when using dynamic row heights.
        /// </summary>
        /// <param name="entity">The entity for a cell.</param>
        /// <returns>The height estimate for a cell.</returns>
        protected virtual nfloat EstimateHeightForCell(TEntity entity)
        {
            return Constants.DefaultEstimatedCellHeight;
        }

        /// <summary>
        /// Occurs when a row has been selected.
        /// </summary>
        /// <param name="cell">The cell that was selected.</param>
        protected virtual void OnRowSelected(UIEntityTableViewCell<TEntity> cell)
        {
        }

        /// <summary>
        /// Occurs when a row has been deselected. 
        /// </summary>
        /// <param name="cell">The cell that was deselected.</param>
        protected virtual void OnRowDeselected(UIEntityTableViewCell<TEntity> cell)
        {
        }

        /// <summary>
        /// Attempts to get the image that should be displayed with the specified cell if it is available locally.
        /// </summary>
        /// <param name="cell">The cell to get the image for.</param>
        /// <returns>The image for the cell or null if an image is not currently available.</returns>
        protected virtual UIImage TryGetImageForCell(UIEntityTableViewCell<TEntity> cell)
        {
            if (this.imageCatalog == null)
            {
                throw new InvalidOperationException("A remote image catalog has not been set so remote image downloading is not available.");
            }

            return 
                this.imageCatalog.TryGetImage(
                    cell.ImageUrl, 
                    cell.ImageOptions,
                    bytes => UIImageByteConverter.FromBytes(bytes, decompress: true));
        }

        /// <summary>
        /// Gets an image to display in a cell when a download failed due to the network being disconnected.
        /// </summary>
        /// <returns>The disconnected image or null if an image should nots be displayed.</returns>
        protected virtual UIImage GetDisconnectedImage()
        {
            return null;
        }

        /// <summary>
        /// Gets an image to display when an image failed to download due to an error other than the network being disconnected.
        /// </summary>
        /// <param name="failedResult">The service result identifying the failure.</param>
        /// <returns>The image for a failed download or null if an image should not be displayed.</returns>
        protected virtual UIImage GetImageDownloadFailedImage(ServiceResult failedResult)
        {
            return null;
        }

        /// <summary>
        /// Creates a new cell responsible for displaying an indicator that more pages are being loaded.
        /// </summary>
        /// <returns>A new table view cell.</returns>
        protected virtual UITableViewCell CreateLoadingMorePagesCell()
        {
            ActivityIndicatorCell cell = new ActivityIndicatorCell();
            cell.StartAnimating();
            return cell;
        }

        /// <summary>
        /// Handles when the table view needs to be refreshed. The table will be refreshed
        /// immediately if the current view is visible on screen, otherwise it will be
        /// delayed until it appears.
        /// </summary>
        protected void HandleNeedsRefreshed()
        {
            if (this.isShown)
            {
                this.StartTableRefresh();
            }
            else
            {
                this.isRefreshNeeded = true;
            }
        }

        /// <summary>
        /// Reloads the table with the current list of items without doing a server-side refresh request.
        /// </summary>
        protected void ReloadTable()
        {
            this.TableView.BeginUpdates();

            this.uiTableViewSource.UpdateItems(this.tableSource.Items);
            this.UpdateTableState();

            this.TableView.EndUpdates();
        }

        /// <summary>
        /// Performs an async refresh of the table by pulling updated information from a service and reloading the data in the current table.
        /// </summary>
        /// <returns>The refresh task.</returns>
        protected Task RefreshTable()
        {
            return this.tableSource.RefreshItemsAsync(this.ReloadTable);
        }

        /// <summary>
        /// Performs an async refresh of the table by pulling updated information from a service and reloading the data in the current table.
        /// </summary>
        /// <param name="loadingOverlay">An optional loading overlay to display while refreshing the table.</param>
        /// <returns>The refresh task.</returns>
        protected async Task RefreshTable(ILoadingOverlay loadingOverlay)
        {
            using (loadingOverlay.ShowLoadingOverlay())
            {
                await this.tableSource.RefreshItemsAsync(this.ReloadTable);
            }
        }

        /// <summary>
        /// Creates a background view to show when the table is empty.
        /// </summary>
        /// <returns>The background view or null if no background should be shown.</returns>
        protected virtual UIView CreateBackgroundView()
        {
            return null;
        }

        private async Task StartTableRefresh()
        {
            this.ShowRefreshIndicator();
            await this.RefreshTable();
            this.HideRefreshIndicator();
        }

        private void InitializeManualRefreshControl()
        {
            this.manualRefreshControl = new UIRefreshControl();
            this.manualRefreshControl.AddTarget(
                async (sender, e) =>
                {
                    this.HideRefreshIndicator();
                    await this.StartTableRefresh();
                    this.manualRefreshControl.EndRefreshing();
                },
                UIControlEvent.ValueChanged);

            if (this.manualRefreshTintColor != null)
            {
                this.manualRefreshControl.TintColor = this.manualRefreshTintColor;
            }

            if (this.manualRefreshBackgroundColor != null)
            {
                this.manualRefreshControl.BackgroundColor = this.manualRefreshBackgroundColor;
            }
        }

        private void InitializeRefreshIndicator()
        {
            this.refreshIndicator = new UILabel();

            this.refreshIndicator.Text = this.refreshIndicatorText;
            this.refreshIndicator.TextAlignment = UITextAlignment.Center;

            if (this.refreshIndicatorTheme != null)
            {
                this.refreshIndicator.ApplyTheme(this.refreshIndicatorTheme);
            }
        }

        private void ShowRefreshIndicator()
        {
            if (!this.isRefreshIndicatorShown)
            {
                ViewAnimation.AnimateMoveVertical(
                    Constants.RefreshIndicatorAnimationDuration,
                    Constants.RefreshIndicatorHeight,
                    this.refreshIndicator);

                this.isRefreshIndicatorShown = true;
            }
        }

        private void HideRefreshIndicator()
        {
            if (this.isRefreshIndicatorShown)
            {
                ViewAnimation.ResetTransform(this.refreshIndicator);
                this.ResetRefreshIndicator();
            }
        }

        private void ResetRefreshIndicator()
        {
            if (this.refreshIndicator != null)
            {
                this.isRefreshIndicatorShown = false;
                this.refreshIndicator.Frame = 
                    new CGRect(
                        0,
                        -Constants.RefreshIndicatorHeight,
                        this.TableView.Frame.Width,
                        Constants.RefreshIndicatorHeight);
            }
        }

        private void UpdateTableState()
        {
            if (this.TableView.BackgroundView != null && this.tableSource.Items.Any())
            {
                this.TableView.BackgroundView = null;
            }
            else if (this.TableView.BackgroundView == null && !this.tableSource.Items.Any())
            {
                this.TableView.BackgroundView = this.CreateBackgroundView();
            }

            this.UpdateManualRefreshControlState();
        }

        private void UpdateManualRefreshControlState()
        {
            if (this.allowManualRefresh &&
                this.TableView.BackgroundView != null &&
                this.tableSource.Items.Any())
            {
                this.EnableManualRefreshControl();
            }
            else
            {
                this.DisableManualRefreshControl();
            }
        }

        private void EnableManualRefreshControl()
        {
            if (this.manualRefreshControl != null && this.manualRefreshControl.Superview == null)
            {
                this.TableView.AddSubview(this.manualRefreshControl);
                this.TableView.SendSubviewToBack(this.manualRefreshControl);
            }
        }

        private void DisableManualRefreshControl()
        {
            if (this.manualRefreshControl != null && this.manualRefreshControl.Superview != null)
            {
                this.manualRefreshControl.RemoveFromSuperview();
            }
        }

        private void StartLoadingNextPageIfNeeded(NSIndexPath cellBeingDisplayed)
        {
            if (this.tableSource.HasMorePages &&
                !this.isLoadingNextPage)
            {
                int totalItems = this.tableSource.Items.Count();

                if (this.uiTableViewSource.WasScrolledDown ||
                    cellBeingDisplayed.Row == totalItems)
                {
                    if ((totalItems - cellBeingDisplayed.Row) < (this.tableSource.PageSize / 2))
                    {
                        this.StartLoadingNextPage();
                    }
                }
            }
        }

        private async void StartLoadingNextPage()
        {
            if (this.isLoadingNextPage)
            {
                throw new InvalidOperationException("A loading task is already in progress.");
            }

            this.isLoadingNextPage = true;

            if (this.imageCatalog != null)
            {
                await this.imageCatalog.GetImageDownloadsInProgressTask();
            }

            await this.tableSource.LoadNextPageAsync(
                () =>
                {
                    UIView.AnimationsEnabled = false;
                    this.TableView.BeginUpdates();

                    this.uiTableViewSource.InsertNewPage(this.tableSource.Items, this.tableSource.HasMorePages);
                    this.TableView.LayoutIfNeeded();

                    this.TableView.EndUpdates();
                    UIView.AnimationsEnabled = true;
                });

            this.isLoadingNextPage = false;
        }

        private async void RequestImageForCell(UIEntityTableViewCell<TEntity> cell, NSIndexPath indexPath)
        {
            UIImage image = this.TryGetImageForCell(cell);

            if (image != null)
            {
                cell.SetImage(image);
            }
            else
            {
                string imageUrl = cell.ImageUrl;
                RemoteImageRequest imageRequest = this.imageCatalog.StartImageRequest(imageUrl, cell.ImageOptions);

                if (!imageRequest.IsRequestCompleted)
                {
                    cell.SetImageDownloadInProgress();
                }

                ServiceResult result = await imageRequest.Task;

                // Check if the requesting cell was new and ensure that it hasn't already been reused.
                if (indexPath.Length == 0 && imageUrl == cell.ImageUrl)
                {
                    this.SetImageFromResult(result, cell);
                }
                else
                {
                    this.SetImageFromResult(result, indexPath);
                }
            }
        }

        private void SetImageFromResult(ServiceResult result, NSIndexPath indexPath)
        {
            if (!this.isDisposed)
            {
                UIEntityTableViewCell<TEntity> cell = this.TableView.CellAt(indexPath) as UIEntityTableViewCell<TEntity>;
                this.SetImageFromResult(result, cell);
            }
        }

        private void SetImageFromResult(ServiceResult result, UIEntityTableViewCell<TEntity> cell)
        {
            if (cell != null)
            {
                if (result.ResultCode == ServiceResultCode.Success)
                {
                    UIImage image = this.TryGetImageForCell(cell);

                    if (image == null)
                    {
                        // is it possible the image was automatically removed from the cache in such a short period of time?
                        throw new InvalidOperationException("An image was not found in the cache.");
                    }

                    cell.SetImage(image);
                }
                else 
                {
                    UIImage errorImage =
                        result.ResultCode == ServiceResultCode.NetworkUnavailable
                        ? this.GetDisconnectedImage()
                        : this.GetImageDownloadFailedImage(result);

                    UIViewContentMode contentMode = UIViewContentMode.Center;
                    UIImageView imageView = cell.GetImageView();

                    if (errorImage != null &&
                        (errorImage.Size.Width > imageView.Bounds.Width ||
                         errorImage.Size.Height > imageView.Bounds.Height))
                    {
                        contentMode = UIViewContentMode.ScaleAspectFit;
                    }

                    cell.SetImage(errorImage, contentMode);
                }

                cell.MarkImageDownloadComplete(result.ResultCode == ServiceResultCode.Success);
            }
        }

        private class TableViewSourceWithoutPages : IPageableTableViewSource<TEntity>
        {
            private readonly ITableViewSource<TEntity> tableViewSource;

            public TableViewSourceWithoutPages(ITableViewSource<TEntity> tableViewSource)
            {
                this.tableViewSource = tableViewSource;
            }

            public IEnumerable<TEntity> Items => this.tableViewSource.Items;
            public int PageSize => int.MaxValue;
            public bool HasMorePages => false;

            public Task LoadNextPageAsync(Action onSuccess)
            {
                throw new NotSupportedException();
            }

            public Task RefreshItemsAsync(Action onSuccess)
            {
                return this.tableViewSource.RefreshItemsAsync(onSuccess);
            }
        }

        private class ActivityIndicatorCell : UITableViewCell
        {
            private UIActivityIndicatorView indicator;

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (this.indicator != null)
                {
                    this.indicator.Center = new CGPoint(this.Bounds.Width / 2, this.Bounds.Height / 2);
                }
            }

            public void StartAnimating()
            {
                if (this.indicator == null)
                {
                    this.indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                    this.AddSubview(indicator);
                    indicator.StartAnimating();
                }
            }
        }
    }
}