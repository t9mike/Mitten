using System;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Base class for handling animations based on keyboard layout events.
    /// </summary>
    public abstract class KeyboardLayoutAnimation
    {
        private UIViewController viewController;

        /// <summary>
        /// Sets the parent view controller.
        /// </summary>
        /// <param name="viewController">The parent view controller.</param>
        public void SetParent(UIViewController viewController)
        {
            if (this.viewController != null)
            {
                throw new InvalidOperationException("A parent view controller has already been set.");
            }

            this.viewController = viewController;
        }

        /// <summary>
        /// Handles view animations when the keyboard is shown.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public abstract void AnimateOnShown(UIKeyboardEventArgs e);

        /// <summary>
        /// Handles view animations when the keyboard is hidden.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public abstract void AnimateOnHidden(UIKeyboardEventArgs e);

        /// <summary>
        /// Gets the height of the navigation bar for the current screen.
        /// </summary>
        /// <returns>The navigation bar height.</returns>
        protected nfloat GetNavigationBarHeight()
        {
            return
                this.viewController.NavigationController.NavigationBar.Frame.Height +
                this.viewController.NavigationController.NavigationBar.Frame.Y;
        }
    }
}