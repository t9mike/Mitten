using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Mitten.Mobile.iOS.Views;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Layout animation that splits all the views into 2 sections and hides the top most section if there is not even
    /// room on the screen when the keyboard is shown.
    /// </summary>
    public class KeyboardLayoutHideSectionAnimation : KeyboardLayoutAnimation
    {
        private readonly ViewSection topSection;
        private readonly ViewSection bottomSection;
        private readonly int bottomPadding;

        private bool shouldAnimateOnHide;

        /// <summary>
        /// Initializes a new instance of the KeyboardLayoutHideSectionAnimation class.
        /// </summary>
        /// <param name="topViews">A list of views representing the top section of the screen layout and will be hidden based on screen space and keyboard state.</param>
        /// <param name="bottomViews">A list of views that are below the top views and intended to be moved based on the keyboard state.</param>
        /// <param name="bottomPadding">An optional amount of padding to apply to the bottom of the views when moving them vertical.</param>
        public KeyboardLayoutHideSectionAnimation(
            IEnumerable<UIView> topViews, 
            IEnumerable<UIView> bottomViews, 
            int bottomPadding = 0)
        {
            Throw.IfArgumentNull(topViews, "topViews");
            Throw.IfArgumentNull(bottomViews, "bottomViews");

            this.topSection = new ViewSection(topViews);
            this.bottomSection = new ViewSection(bottomViews);

            this.bottomPadding = bottomPadding;
        }

        /// <summary>
        /// Handles view animations when the keyboard is shown.
        /// </summary>
        /// <param name="e">The keyboard arguments for the event.</param>
        public override void AnimateOnShown(UIKeyboardEventArgs e)
        {
            bool shouldHideTopSection;
            int translateY = this.CalculateVerticalTranslation(e.Notification, out shouldHideTopSection);

            if (translateY < 0)
            {
                Action topSectionAnimation;

                if (shouldHideTopSection)
                {
                    topSectionAnimation = () => this.topSection.Hide();
                }
                else
                {
                    topSectionAnimation = () => ViewAnimation.AnimateMoveVertical(e.AnimationDuration, translateY, this.topSection.Views);
                }

                ViewAnimation.AnimateMoveVertical(
                    e.AnimationDuration,
                    translateY,
                    topSectionAnimation,
                    this.bottomSection.Views);

                shouldAnimateOnHide = true;
            }
            else
            {
                shouldAnimateOnHide = false;
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
                UIView.Animate(
                    e.AnimationDuration,
                    () =>
                {
                    Action topSectionAnimation = this.GetTopViewKeyboardHiddenAnimationCallback(e.AnimationDuration);    
                    topSectionAnimation();

                    ViewAnimation.ResetTransform(this.bottomSection.Views);
                });
            }

            shouldAnimateOnHide = false;
        }

        private nfloat GetCombinedSectionHeight()
        {
            return
                this.bottomSection.BottomView.Frame.Bottom -
                this.topSection.TopView.Frame.Top;
        }

        private Action GetTopViewKeyboardHiddenAnimationCallback(double duration)
        {
            if (this.topSection.IsHidden)
            {
                return () => ViewAnimation.AnimateFadeIn(this.topSection.Views, duration);
            }

            return () => ViewAnimation.ResetTransform(this.topSection.Views);
        }

        private int CalculateVerticalTranslation(NSNotification notification, out bool shouldHideTopSection)
        {
            nfloat screenHeight = UIScreen.MainScreen.Bounds.Height;
            nfloat navigationBarHeight = this.GetNavigationBarHeight();

            NSValue keyboardFrame = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            nfloat keyboardHeight = keyboardFrame.CGRectValue.Height;

            nfloat availableSpace = screenHeight - keyboardHeight - navigationBarHeight - this.bottomPadding;
            nfloat fieldGroupHeight = this.GetCombinedSectionHeight();

            ViewSection topMostVisibleSection = this.topSection;

            if (fieldGroupHeight > availableSpace)
            {
                shouldHideTopSection = true;
                topMostVisibleSection = this.bottomSection;
                fieldGroupHeight = this.bottomSection.Height;
            }
            else
            {
                shouldHideTopSection = false;
            }

            nfloat offset = 0;

            if (fieldGroupHeight < availableSpace)
            {
                offset = (availableSpace - fieldGroupHeight) / 2.0f - this.bottomPadding;
            }

            nfloat distanceFromTop = navigationBarHeight + offset;
            return (int)Math.Ceiling(distanceFromTop - topMostVisibleSection.TopView.Frame.Top);
        }

        private class ViewSection
        {
            public ViewSection(IEnumerable<UIView> views)
            {
                this.Views = views;

                this.TopView = this.GetTopMostView();
                this.BottomView = this.GetBottomMostView();
            }

            public IEnumerable<UIView> Views { get; private set; }
            public UIView TopView { get; private set; }
            public UIView BottomView { get; private set; }

            public nfloat Height 
            { 
                get { return this.BottomView.Frame.Bottom - this.TopView.Frame.Top; }
            }

            public bool IsHidden
            {
                get { return this.TopView.Alpha == 0; }
            }

            public void Hide()
            {
                foreach (UIView view in this.Views)
                {
                    view.Alpha = 0;
                }
            }

            private UIView GetTopMostView()
            {
                UIView topView = this.Views.First();

                foreach (UIView view in this.Views.Skip(1))
                {
                    if (view.Frame.Top < topView.Frame.Top)
                    {
                        topView = view;
                    }
                }

                return topView;
            }

            private UIView GetBottomMostView()
            {
                UIView bottomView = this.Views.First();

                foreach (UIView view in this.Views.Skip(1))
                {
                    if (view.Frame.Bottom > bottomView.Frame.Bottom)
                    {
                        bottomView = view;
                    }
                }

                return bottomView;
            }
        }
    }
}