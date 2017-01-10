using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains helper methods for performing view animations.
    /// </summary>
    public static class ViewAnimation
    {
        /// <summary>
        /// Animates a fade-in where the view starts hidden and fades visible.
        /// </summary>
        /// <param name="view">The view to animate.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="delay">An optional delay before starting the animation.</param>
        /// <param name="animationOptions">The animation options, the default is a cross dissolve transition.</param>
        public static void AnimateFadeIn(
            UIView view, 
            double duration, 
            double delay = 0,
            UIViewAnimationOptions animationOptions = UIViewAnimationOptions.TransitionCrossDissolve)
        {
            ViewAnimation.AnimateFadeIn(new[] { view }, duration, delay, animationOptions);
        }

        /// <summary>
        /// Animates a fade-in where the view starts hidden and fades visible.
        /// </summary>
        /// <param name="views">A list of views to animate.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="delay">An optional delay before starting the animation.</param>
        /// <param name="animationOptions">The animation options, the default is a cross dissolve transition.</param>
        public static void AnimateFadeIn(
            IEnumerable<UIView> views, 
            double duration, 
            double delay = 0,
            UIViewAnimationOptions animationOptions = UIViewAnimationOptions.TransitionCrossDissolve)
        {
            if (delay > 0)
            {
                foreach (UIView view in views)
                {
                    view.Alpha = 0;
                }

                NSTimer.CreateScheduledTimer(delay, timer => ViewAnimation.AnimateFadeIn(views, duration, animationOptions));
            }
            else
            {
                ViewAnimation.AnimateFadeIn(views, duration, animationOptions);
            }
        }

        /// <summary>
        /// Animates a fade-out where the view starts visible and fades to hidden.
        /// </summary>
        /// <param name="view">The view to animate.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="animationOptions">The animation options, the default is a cross dissolve transition.</param>
        public static void AnimateFadeOut(
            UIView view, 
            double duration, 
            UIViewAnimationOptions animationOptions = UIViewAnimationOptions.TransitionCrossDissolve)
        {
            ViewAnimation.AnimateFadeOut(new[] { view }, duration, animationOptions);
        }

        /// <summary>
        /// Animates a fade-out where the view starts visible and fades to hidden.
        /// </summary>
        /// <param name="views">A list of views to animate.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="animationOptions">The animation options, the default is a cross dissolve transition.</param>
        public static void AnimateFadeOut(
            IEnumerable<UIView> views,
            double duration, 
            UIViewAnimationOptions animationOptions = UIViewAnimationOptions.TransitionCrossDissolve)
        {
            foreach (UIView view in views)
            {
                UIView.Animate(
                    duration, 
                    0.0,
                    animationOptions, 
                    () => view.Alpha = 0,
                    () => { });
            }
        }

        /// <summary>
        /// Animates the movement of a list of views in a vertical direction.
        /// </summary>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="translateY">The amount to translate.</param>
        /// <param name="views">One or more views to move vertically.</param>
        public static void AnimateMoveVertical(double duration, nfloat translateY, params UIView[] views)
        {
            ViewAnimation.AnimateMoveVertical(duration, translateY, (IEnumerable<UIView>)views);
        }

        /// <summary>
        /// Animates the movement of a list of views in a vertical direction.
        /// </summary>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="translateY">The amount to translate.</param>
        /// <param name="views">One or more views to move vertically.</param>
        public static void AnimateMoveVertical(double duration, nfloat translateY, IEnumerable<UIView> views)
        {
            ViewAnimation.AnimateMoveVertical(duration, translateY, () => { }, views);
        }

        /// <summary>
        /// Animates the movement of a list of views in a vertical direction.
        /// </summary>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="translateY">The amount to translate.</param>
        /// <param name="animateWith">An optional action that will occur along with this animation.</param>
        /// <param name="views">One or more views to move vertically.</param>
        public static void AnimateMoveVertical(double duration, nfloat translateY, Action animateWith, params UIView[] views)
        {
            ViewAnimation.AnimateMoveVertical(duration, translateY, animateWith, (IEnumerable<UIView>)views);
        }

        /// <summary>
        /// Animates the movement of a list of views in a vertical direction.
        /// </summary>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="translateY">The amount to translate.</param>
        /// <param name="animateWith">An optional action that will occur along with this animation.</param>
        /// <param name="views">One or more views to move vertically.</param>
        public static void AnimateMoveVertical(double duration, nfloat translateY, Action animateWith, IEnumerable<UIView> views)
        {
            UIView.Animate(
                duration,
                () =>
                {
                    animateWith();

                    foreach (UIView view in views)
                    {
                        CGAffineTransform transform = new CGAffineTransform(1, 0, 0, 1, 0, translateY);
                        view.Transform = transform;
                    }
                });
        }

        /// <summary>
        /// Resets the tranform for the given views.
        /// </summary>
        /// <param name="views">One or more views.</param>
        public static void ResetTransform(params UIView[] views)
        {
            ViewAnimation.ResetTransform((IEnumerable<UIView>)views);
        }

        /// <summary>
        /// Resets the tranform for the given views.
        /// </summary>
        /// <param name="views">One or more views.</param>
        public static void ResetTransform(IEnumerable<UIView> views)
        {
            foreach (UIView view in views)
            {
                view.Transform = CGAffineTransform.MakeIdentity();
            }
        }

        private static void AnimateFadeIn(IEnumerable<UIView> views, double duration, UIViewAnimationOptions animationOptions)
        {
            foreach (UIView view in views)
            {
                view.Alpha = 0;

                UIView.Animate(
                    duration, 
                    0.0,
                    animationOptions, 
                    () => view.Alpha = 1,
                    () => { });
            }
        }
    }
}