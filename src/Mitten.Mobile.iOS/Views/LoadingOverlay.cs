using System;
using Mitten.Mobile.Application;
using Mitten.Mobile.Graphics;
using Mitten.Mobile.Themes;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Represents a loading overlay for an iOS app.
    /// </summary>
    public class LoadingOverlay : ILoadingOverlay
    {
        private readonly UIView view;

        /// <summary>
        /// Initializes a new instance of the LoadingOverlay class.
        /// </summary>
        /// <param name="view">A view to display as the loading overlay.</param>
        public LoadingOverlay(UIView view)
        {
            Throw.IfArgumentNull(view, nameof(view));
            this.view = view;
        }

        /// <summary>
        /// Shows a loading overlay on top of the current screen.
        /// </summary>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <returns>An object that will be disposed when the overlay should be hidden and destroyed.</returns>
        public IDisposable ShowLoadingOverlay(IProgress progress = null)
        {
            UIWindow window = new UIWindow();

            window.BackgroundColor = UIColor.Clear;
            window.Frame = UIScreen.MainScreen.Bounds;
            window.WindowLevel = UIWindowLevel.StatusBar;

            window.AddSubview(this.view);

            window.Hidden = false;

            return new CloseWindowHandler(window);
        }

        /// <summary>
        /// Creates a new standard overlay that will cover the entire screen.
        /// </summary>
        /// <param name="spinnerColor">The color for the spinner.</param>
        /// <param name="spinnerBackgroundColor">A background color for the spinner if the screen background color is not opaque.</param>
        /// <param name="screenBackgroundColor">The background color for the screen overlay.</param>
        /// <param name="delayInSeconds">The delay, in seconds, before the overlay becomes visible. The default is 0.5 seconds.</param>
        /// <returns>A new overlay to show on top of the current view.</returns>
        public static LoadingOverlay CreateFullScreenOverlay(
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
        {
            return
                new LoadingOverlay(
                    LoadingOverlayView.CreateFullScreenOverlay(
                        null,
                        null,
                        Colors.Transparent,
                        spinnerColor,
                        spinnerBackgroundColor,
                        screenBackgroundColor,
                        delayInSeconds));
        }

        /// <summary>
        /// Creates a new standard overlay that will cover the entire screen.
        /// </summary>
        /// <param name="progress">Provides progress for the operation the overlay represents.</param>
        /// <param name="progressLabelTheme">A theme for the progress label.</param>
        /// <param name="progressBarColor">The color for the progress bar.</param>
        /// <param name="spinnerColor">The color for the spinner.</param>
        /// <param name="spinnerBackgroundColor">A background color for the spinner if the screen background color is not opaque.</param>
        /// <param name="screenBackgroundColor">The background color for the screen overlay.</param>
        /// <param name="delayInSeconds">The delay, in seconds, before the overlay becomes visible. The default is 0.5 seconds.</param>
        /// <returns>A new overlay to show on top of the current view.</returns>
        public static LoadingOverlay CreateFullScreenOverlay(
            IProgress progress,
            LabelTheme progressLabelTheme,
            Color progressBarColor,
            Color spinnerColor,
            Color spinnerBackgroundColor,
            Color screenBackgroundColor,
            float delayInSeconds)
        {
            return
                new LoadingOverlay(
                    LoadingOverlayView.CreateFullScreenOverlay(
                        progress,
                        progressLabelTheme,
                        progressBarColor,
                        spinnerColor,
                        spinnerBackgroundColor,
                        screenBackgroundColor,
                        delayInSeconds));
        }
    }
}