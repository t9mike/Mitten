using System;
using System.Collections.Generic;
using CoreGraphics;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Handles adjusting the screen layout based on the device size and keyboard state.
    /// </summary>
    public class KeyboardLayoutHandler<TViewModel>
        where TViewModel : ViewModel
    {
        private readonly UIViewController<TViewModel> viewController;
        private bool isKeyboardShown;

        /// <summary>
        /// Initializes a new instance of the KeyboardLayoutHandler class.
        /// </summary>
        /// <param name="viewController">The view controller.</param>
        public KeyboardLayoutHandler(UIViewController<TViewModel> viewController)
        {
            this.viewController = viewController;
        }

        /// <summary>
        /// Initializes the handler to use a section hiding animation.
        /// </summary>
        /// <param name="topViews">A list of views representing the top section of the screen layout and will be hidden based on screen space and keyboard state.</param>
        /// <param name="bottomViews">A list of views that are below the top views and intended to be moved based on the keyboard state.</param>
        /// <param name="bottomPadding">An optional amount of padding to apply to the bottom of the views when moving them vertical.</param>
        public void InitializeWithHidingAnimation(IEnumerable<UIView> topViews, IEnumerable<UIView> bottomViews, int bottomPadding = 0)
        {
            this.Initialize(new KeyboardLayoutHideSectionAnimation(topViews, bottomViews, bottomPadding));
        }

        /// <summary>
        /// Initializes the handler to use a vertical move animation.
        /// </summary>
        /// <param name="view">The view that will be moved.</param>
        /// <param name="bottomPadding">The amount of padding between the top of the keyboard and the bottom of the view.</param>
        public void InitializeWithMoveAnimation(UIView view, int bottomPadding = 0)
        {
            this.Initialize(new KeyboardLayoutMoveAnimation(view, bottomPadding));
        }

        /// <summary>
        /// Initializes the handler to use custom animations.
        /// </summary>
        /// <param name="animateShown">Handles animation when the keyboard is shown.</param>
        /// <param name="animateHidden">Handles animation when the keyboard is hidden.</param>
        public void InitializeWithCustomAnimation(Func<CGRect, double, bool> animateShown, Action<double> animateHidden)
        {
            this.Initialize(new KeyboardLayoutCustomAnimation(animateShown, animateHidden));
        }

        private void Initialize(KeyboardLayoutAnimation layoutAnimation)
        {
            Throw.IfArgumentNull(layoutAnimation, nameof(layoutAnimation));

            layoutAnimation.SetParent(this.viewController);

            this.viewController.RegisterKeyboardWillBeShown(e => this.OnKeyboardWillBeShown(e, layoutAnimation.AnimateOnShown));
            this.viewController.RegisterKeyboardWillBeHidden(e => this.OnKeyboardWillBeHidden(e, layoutAnimation.AnimateOnHidden));
        }

        private void OnKeyboardWillBeShown(UIKeyboardEventArgs e, Action<UIKeyboardEventArgs> animateViews)
        {
            // this will get called if a keyboard is already shown but changed
            // to a different keyboard, e.g. password keyboard

            if (!this.isKeyboardShown)
            {
                this.isKeyboardShown = true;
                animateViews(e);
            }
        }

        private void OnKeyboardWillBeHidden(UIKeyboardEventArgs e, Action<UIKeyboardEventArgs> animateViews)
        {
            this.isKeyboardShown = false;
            animateViews(e);
        }
    }
}