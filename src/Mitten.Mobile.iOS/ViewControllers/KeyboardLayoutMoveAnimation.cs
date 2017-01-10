using System;
using Foundation;
using Mitten.Mobile.iOS.Views;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Layout animation that will move a view vertically.
    /// </summary>
    public class KeyboardLayoutMoveAnimation : KeyboardLayoutAnimation
    {
        private readonly UIView view;
        private readonly int bottomPadding;

        private bool shouldAnimateOnHide;

        /// <summary>
        /// Initializes a new instance of the KeyboardLayoutMoveAnimation class.
        /// </summary>
        /// <param name="view">The view that will be moved.</param>
        /// <param name="bottomPadding">The amount of padding between the top of the keyboard and the bottom of the view.</param>
        public KeyboardLayoutMoveAnimation(UIView view, int bottomPadding)
        {
            Throw.IfArgumentNull(view, nameof(view));

            this.view = view;
            this.bottomPadding = bottomPadding;
        }

        /// <summary>
        /// Handles view animations when the keyboard is shown.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public override void AnimateOnShown(UIKeyboardEventArgs e)
        {
            NSValue keyboardFrame = (NSValue)e.Notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            nfloat maximumViewBottom = keyboardFrame.CGRectValue.Top - this.bottomPadding;

            if (this.view.Frame.Bottom > maximumViewBottom)
            {
                nfloat amountToTranslate = this.view.Frame.Bottom - maximumViewBottom;
                ViewAnimation.AnimateMoveVertical(e.AnimationDuration, -amountToTranslate, () => this.view.Superview.LayoutIfNeeded(), this.view);
                this.shouldAnimateOnHide = true;
            }
        }

        /// <summary>
        /// Handles view animations when the keyboard is hidden.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public override void AnimateOnHidden(UIKeyboardEventArgs e)
        {
            if (shouldAnimateOnHide)
            {
                UIView.Animate(e.AnimationDuration, () => ViewAnimation.ResetTransform(this.view));
            }

            shouldAnimateOnHide = false;
        }

        private nfloat CalculateNewHeight(NSNotification notification)
        {
            NSValue keyboardFrame = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            nfloat keyboardTop = keyboardFrame.CGRectValue.Top;

            if (keyboardTop >= this.view.Frame.Bottom)
            {
                return this.view.Frame.Height;
            }

            nfloat diff = this.view.Frame.Bottom - keyboardTop + this.bottomPadding;
            return this.view.Frame.Height - diff;
        }
    }
}