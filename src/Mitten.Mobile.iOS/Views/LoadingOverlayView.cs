using System;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.Application;
using Mitten.Mobile.Graphics;
using Mitten.Mobile.iOS.Extensions;
using Mitten.Mobile.iOS.Views.Renderers;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// A standard overlay view that shows a loading indicator with support for a progress bar.
    /// </summary>
    public class LoadingOverlayView : UIView
    {
        private static class Constants
        {
            public const int ActivitySpinnerSize = 50;
            public const float ActivitySpinnerHalfSize = ActivitySpinnerSize / 2;

            public const int DescriptionLabelPadding = 16;

            public const int ProgressBarHeight = 4;
            public const int ProgressBarWidth = 200;

            public const int VerticalMargins = 8;

            public const int SpinnerBackgroundPadding = 24;
            public const int SpinnerBackgroundCornerRadius = 16;
        }

        private LoadingOverlayView(
            CGRect frame,
            IProgress progress,
            LabelTheme progressLabelTheme,
            Color progressBarColor,
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
            : base(frame)
        {
            this.Initialize(
                progress,
                progressLabelTheme,
                progressBarColor,
                spinnerColor,
                spinnerBackgroundColor,
                screenBackgroundColor,
                delayInSeconds);
        }

        /// <summary>
        /// Hides the overlay.
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5f,
                () => this.Alpha = 0,
                this.RemoveFromSuperview);
        }

        /// <summary>
        /// Creates a new overlay that will cover the entire screen.
        /// </summary>
        /// <param name="spinnerColor">The color for the spinner.</param>
        /// <param name="spinnerBackgroundColor">A background color for the spinner if the screen background color is not opaque.</param>
        /// <param name="screenBackgroundColor">The background color for the screen overlay.</param>
        /// <param name="delayInSeconds">The delay, in seconds, before the overlay becomes visible. The default is 0.5 seconds.</param>
        /// <returns>A new overlay to show on top of the current view.</returns>
        public static LoadingOverlayView CreateFullScreenOverlay(
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
        {
            return
                new LoadingOverlayView(
                    UIScreen.MainScreen.Bounds,
                    null,
                    null,
                    Colors.Transparent,
                    spinnerColor,
                    spinnerBackgroundColor,
                    screenBackgroundColor,
                    delayInSeconds);
        }

        /// <summary>
        /// Creates a new overlay that will cover the entire screen.
        /// </summary>
        /// <param name="progress">Provides progress for the operation the overlay represents.</param>
        /// <param name="progressLabelTheme">A theme for the progress label.</param>
        /// <param name="progressBarColor">The color for the progress bar.</param>
        /// <param name="spinnerColor">The color for the spinner.</param>
        /// <param name="spinnerBackgroundColor">A background color for the spinner if the screen background color is not opaque.</param>
        /// <param name="screenBackgroundColor">The background color for the screen overlay.</param>
        /// <param name="delayInSeconds">The delay, in seconds, before the overlay becomes visible. The default is 0.5 seconds.</param>
        /// <returns>A new overlay to show on top of the current view.</returns>
        public static LoadingOverlayView CreateFullScreenOverlay(
            IProgress progress,
            LabelTheme progressLabelTheme,
            Color progressBarColor,
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
        {
            return
                new LoadingOverlayView(
                    UIScreen.MainScreen.Bounds,
                    progress,
                    progressLabelTheme,
                    progressBarColor,
                    spinnerColor,
                    spinnerBackgroundColor,
                    screenBackgroundColor,
                    delayInSeconds);
        }

        private void Initialize(
            IProgress progress,
            LabelTheme progressLabelTheme,
            Color progressBarColor,
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
        {
            this.AutoresizingMask = UIViewAutoresizing.All;
            this.BackgroundColor = UIColor.Clear;

            NSTimer.CreateScheduledTimer(
                delayInSeconds,
                timer =>
                {
                    this.BackgroundColor = screenBackgroundColor.ToUIColor();
                    this.InitializeActivitySpinner(spinnerColor.ToUIColor());

                    if (progress != null)
                    {
                        this.InitializeProgressView(progress, progressLabelTheme, progressBarColor);
                    }

                    if (screenBackgroundColor.Alpha < 255)
                    {
                        this.InitializeSpinnerBackground(spinnerBackgroundColor.ToUIColor());
                    }
                });
        }

        private void InitializeActivitySpinner(UIColor spinnerColor)
        {
            CGRect activitySpinnerBounds =
                new CGRect(
                    this.Frame.GetMidX() - Constants.ActivitySpinnerHalfSize,
                    this.Frame.GetMidY() - Constants.ActivitySpinnerHalfSize,
                    Constants.ActivitySpinnerSize,
                    Constants.ActivitySpinnerSize);

            UIView activitySpinnerView = new UIView(activitySpinnerBounds);
            ActivitySpinnerRenderer activitySpinnerRenderer = new ActivitySpinnerRenderer(activitySpinnerView, spinnerColor);

            activitySpinnerRenderer.RenderToViewWithAnimation();

            this.AddSubview(activitySpinnerView);
        }

        private void InitializeProgressView(IProgress progress, LabelTheme progressLabelTheme, Color progressBarColor)
        {
            nfloat centerX = this.Frame.GetMidX();
            nfloat centerY = this.Frame.GetMidY();
            nfloat locationY = centerY + Constants.ActivitySpinnerHalfSize + Constants.VerticalMargins;

            if (!string.IsNullOrWhiteSpace(progress.Description))
            {
                UILabel description = new UILabel();

                description.Frame = new CGRect(0, locationY, this.Frame.Width, 0);
                description.Text = progress.Description;
                description.TextAlignment = UITextAlignment.Center;

                description.ApplyTheme(progressLabelTheme, ViewFontSizes.SmallFontSize);
                description.SizeToFit();

                nfloat descriptionWidth = description.Frame.Width + Constants.DescriptionLabelPadding;
                description.Frame =
                    new CGRect(
                        centerX - descriptionWidth / 2,
                        description.Frame.Y,
                        descriptionWidth,
                        description.Frame.Height);

                locationY += description.Frame.Height + Constants.VerticalMargins;

                progress.DescriptionChanged += () => this.InvokeOnMainThread(() => description.Text = progress.Description);

                this.AddSubview(description);
            }

            if (progress.Maximum > 0)
            {
                nfloat progressBarX = centerX - (Constants.ProgressBarWidth / 2);

                UIView progressBarBackground = new UIView();
                progressBarBackground.BackgroundColor = Colors.StormCloud.ToUIColor();
                progressBarBackground.Frame = new CGRect(progressBarX, locationY, Constants.ProgressBarWidth, Constants.ProgressBarHeight);

                UIView progressBar = new UIView();
                progressBar.BackgroundColor = progressBarColor.ToUIColor();
                progressBar.Frame = new CGRect(progressBarX, locationY, 0, Constants.ProgressBarHeight);

                this.AddSubview(progressBarBackground);
                this.AddSubview(progressBar);

                this.BringSubviewToFront(progressBar);

                progress.ValueChanged +=
                    () =>
                    {
                        this.InvokeOnMainThread(
                        () =>
                        {
                            float percentage = (float)progress.Value / progress.Maximum;
                            nfloat width = Constants.ProgressBarWidth * percentage;

                            progressBar.Frame = new CGRect(progressBar.Frame.X, progressBar.Frame.Y, width, Constants.ProgressBarHeight);
                        });
                    };
            }
        }

        private void InitializeSpinnerBackground(UIColor spinnerBackgroundColor)
        {
            UIView spinnerBackground = new UIView();

            spinnerBackground.Frame = this.CalculateSpinnerBackgroundFrame();
            spinnerBackground.BackgroundColor = spinnerBackgroundColor;
            spinnerBackground.Layer.CornerRadius = Constants.SpinnerBackgroundCornerRadius;
            spinnerBackground.Layer.MasksToBounds = true;

            this.AddSubview(spinnerBackground);
            this.SendSubviewToBack(spinnerBackground);
        }

        private CGRect CalculateSpinnerBackgroundFrame()
        {
            this.LayoutIfNeeded();

            CGRect frame = CGRect.Empty;
            nfloat x = nfloat.MaxValue;
            nfloat y = nfloat.MaxValue;

            foreach (UIView view in this.Subviews)
            {
                x = (nfloat)Math.Min(view.Frame.X, x);
                y = (nfloat)Math.Min(view.Frame.Y, y);

                frame = frame.UnionWith(view.Frame);
            }

            return
                new CGRect(
                    x - Constants.SpinnerBackgroundPadding,
                    y - Constants.SpinnerBackgroundPadding,
                    frame.Width - x + Constants.SpinnerBackgroundPadding * 2,
                    frame.Height - y + Constants.SpinnerBackgroundPadding * 2);
        }
    }
}