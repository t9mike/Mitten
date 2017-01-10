using System;
using CoreGraphics;
using Mitten.Mobile.iOS.Views.Renderers;
using Mitten.Mobile.Model;
using Mitten.Mobile.Remote;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Base class for a table cell that displays information for an entity and supports displaying a remote image.
    /// </summary>
    public abstract class UIEntityTableViewCell<TEntity> : UITableViewCell
        where TEntity : Entity
    {
        private static class Constants
        {
            public const int ActivitySpinnerSize = 30;
            public const int ActivitySpinnerLineWidth = 4;

            public const int MaximumImageSize = 2048;
        }

        private UIView activitySpinnerView;

        /// <summary>
        /// Initializes a new instance of the UIEntityTableViewCell class.
        /// </summary>
        /// <param name="handle">Handle.</param>
        protected UIEntityTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.ActivitySpinnerColor = UIColor.White;
            this.ShowActivitySpinnerWhenDownloadingImage = true;
        }

        /// <summary>
        /// Initializes a new instance of the UIEntityTableViewCell class.
        /// </summary>
        /// <param name="style">The style for the cell.</param>
        /// <param name="reuseIdentifier">A reuse identifier.</param>
        protected UIEntityTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(style, reuseIdentifier)
        {
            this.ActivitySpinnerColor = UIColor.White;
            this.ShowActivitySpinnerWhenDownloadingImage = true;
        }

        /// <summary>
        /// Gets whether or not the cell currently has a placeholder image.
        /// </summary>
        public bool HasPlaceHolderImage { get; private set; }

        /// <summary>
        /// Gets whether or not an image download is currently in progress.
        /// </summary>
        public bool IsImageDownloadInProgress { get; private set; }

        /// <summary>
        /// Gets whether or not the image was successfully downloaded, otherwise false if a download failed.
        /// </summary>
        public bool WasImageDownloadedSuccessfully { get; private set; }

        /// <summary>
        /// Gets whether or not this cell supports displaying an image downloaded from a remote server.
        /// </summary>
        public virtual bool IsRemoteImageSupported
        {
            get { return !string.IsNullOrWhiteSpace(this.ImageUrl); }
        }

        /// <summary>
        /// Gets the url for the image to display in the cell.
        /// </summary>
        public virtual string ImageUrl
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the options for the image to display in the cell.
        /// </summary>
        public virtual ImageOptions ImageOptions
        {
            get { return new ImageOptions(Constants.MaximumImageSize, Constants.MaximumImageSize, ImageResizeMode.Default); }
        }

        /// <summary>
        /// Gets the entity for the current cell.
        /// </summary>
        public TEntity Entity { get; private set; }

        /// <summary>
        /// Gets or sets the color to use for the activity spinner, the default is White.
        /// </summary>
        protected UIColor ActivitySpinnerColor { get; set; }

        /// <summary>
        /// Gets or sets whether or not the acitvity spinner should be shown when downloading an image; the default is true.
        /// </summary>
        protected bool ShowActivitySpinnerWhenDownloadingImage { get; set; }

        /// <summary>
        /// Lays out the subviews.
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (this.activitySpinnerView != null)
            {
                this.CenterDownloadProgressIndicator();
            }
        }

        /// <summary>
        /// Initializes the current cell.
        /// </summary>
        /// <param name="entity">An entity to bind to.</param>
        public void Initialize(TEntity entity)
        { 
            this.Entity = entity;
            this.Initialize();

            this.ResetDownloadProgress();

            if (this.IsRemoteImageSupported)
            {
                UIImageView imageView = this.GetImageView();
                imageView.Image = null;

                this.HasPlaceHolderImage = true;
            }
        }

        /// <summary>
        /// Sets the specified image for the cell.
        /// </summary>
        /// <param name="image">The image to set.</param>
        public void SetImage(UIImage image, UIViewContentMode contentMode = UIViewContentMode.ScaleAspectFill)
        { 
            UIImageView imageView = this.GetImageView();

            imageView.ContentMode = contentMode;
            imageView.Image = image;

            this.HasPlaceHolderImage = false;
        }

        /// <summary>
        /// Sets that an image download is in progress and shows a progress indicator.
        /// </summary>
        public void SetImageDownloadInProgress()
        {
            if (this.IsImageDownloadInProgress)
            {
                throw new InvalidOperationException("Image download already in progress.");
            }

            if (this.ShowActivitySpinnerWhenDownloadingImage)
            {
                this.activitySpinnerView = new UIView();
                this.GetImageView().AddSubview(activitySpinnerView);

                this.CenterDownloadProgressIndicator();

                ActivitySpinnerRenderer activitySpinnerRenderer = new ActivitySpinnerRenderer(activitySpinnerView, this.ActivitySpinnerColor, Constants.ActivitySpinnerLineWidth);
                activitySpinnerRenderer.RenderToViewWithAnimation();
            }

            this.IsImageDownloadInProgress = true;
        }

        /// <summary>
        /// Signals that the image download has completed.
        /// </summary>
        /// <param name="wasSuccessful">True if the image was downloaded successfully, otherwise the download completed with a failure.</param>
        public void MarkImageDownloadComplete(bool wasSuccessful)
        {
            this.ResetDownloadProgress();
            this.WasImageDownloadedSuccessfully = wasSuccessful;
        }

        /// <summary>
        /// Gets the image view for the cell.
        /// </summary>
        /// <returns>The image view.</returns>
        public virtual UIImageView GetImageView()
        {
            if (this.IsRemoteImageSupported)
            {
                throw new InvalidOperationException("Subclasses must override the GetImageView method.");
            }

            throw new NotSupportedException("Images for this cell are not supported.");
        }

        /// <summary>
        /// Initializes the cell.
        /// </summary>
        protected abstract void Initialize();

        private void ResetDownloadProgress()
        {
            if (this.IsImageDownloadInProgress && this.activitySpinnerView != null)
            {
                this.activitySpinnerView.RemoveFromSuperview();
                this.activitySpinnerView = null;

                this.IsImageDownloadInProgress = false;
            }
        }

        private void CenterDownloadProgressIndicator()
        {
            UIView superView = this.activitySpinnerView.Superview;
            superView.LayoutIfNeeded();

            this.activitySpinnerView.Frame =
                new CGRect(
                    (superView.Frame.Width / 2 - Constants.ActivitySpinnerSize / 2),
                    (superView.Frame.Height / 2 - Constants.ActivitySpinnerSize / 2),
                    Constants.ActivitySpinnerSize,
                    Constants.ActivitySpinnerSize);
        }
    }
}