using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Layout animation that supports a custom animation callback.
    /// </summary>
    public class KeyboardLayoutCustomAnimation : KeyboardLayoutAnimation
    {
        private readonly Func<CGRect, double, bool> animateShown;
        private readonly Action<double> animateHidden;

        private bool shouldAnimateOnHide;

        /// <summary>
        /// Initializes a new instance of the KeyboardLayoutCustomAnimation class.
        /// </summary>
        /// <param name="animateShown">Handles animation when the keyboard is shown.</param>
        /// <param name="animateHidden">Handles animation when the keyboard is hidden.</param>
        public KeyboardLayoutCustomAnimation(Func<CGRect, double, bool> animateShown, Action<double> animateHidden)
        {
            Throw.IfArgumentNull(animateShown, nameof(animateShown));
            Throw.IfArgumentNull(animateHidden, nameof(animateHidden));

            this.animateShown = animateShown;
            this.animateHidden = animateHidden;
        }

        /// <summary>
        /// Handles view animations when the keyboard is shown.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public override void AnimateOnShown(UIKeyboardEventArgs e)
        {
            NSValue keyboardFrame = (NSValue)e.Notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            this.shouldAnimateOnHide = this.animateShown(keyboardFrame.CGRectValue, e.AnimationDuration);
        }

        /// <summary>
        /// Handles view animations when the keyboard is hidden.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public override void AnimateOnHidden(UIKeyboardEventArgs e)
        {
            if (shouldAnimateOnHide)
            {
                this.animateHidden(e.AnimationDuration);
            }

            shouldAnimateOnHide = false;
        }
    }
}