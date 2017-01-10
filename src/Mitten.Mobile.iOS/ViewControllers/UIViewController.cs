using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Base class for a view controller that is associated with a specified view model.
    /// </summary>
    public abstract class UIViewController<TViewModel> : UIViewController
        where TViewModel : ViewModel
    {
        private static class Constants
        {
            public const int PaddingWhenKeyboardVisible = 8;
        }

        private List<NSObject> notificationObservers;

        private UIViewControllerFactory viewControllerFactory;

        private UIScrollView scrollView;
        private UIEdgeInsets? originalContentInsets;

        private bool ensureNavigationBarVisible;
        private bool ensureNavigationBarHidden;

        /// <summary>
        /// Initializes a new instance of the UIViewController class.
        /// </summary>
        protected UIViewController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UIViewController class.
        /// </summary>
        /// <param name="handle">Object handle used by the runtime.</param>
        protected UIViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Gets the view model for this controller.
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets whether or not the current view controller is a child to another view controller.
        /// </summary>
        public bool IsChild { get; private set; }

        /// <summary>
        /// Gets whether or not the view for this view controller has appeared.
        /// </summary>
        protected bool HasViewAppeared
        {
            get { return this.View.Superview != null; }
        }

        /// <summary>
        /// Occurs when the view has been loaded.
        /// </summary>
        public override async void ViewDidLoad()
        {
            // If there is a navigation bar and it is set to opaque, the main view for this view controller
            // will automatically get pushed down below the navigation bar. However, if the navigation
            // bar is translucent, the view will not get pushed down. Setting this to true prevents the
            // view from getting pushed down when opaque. This way, we can layout our
            // views and not be concerned whether or not there will be a navigation bar.
            this.ExtendedLayoutIncludesOpaqueBars = true;

            base.ViewDidLoad();
            await this.HandleViewDidLoad();

            this.ViewModel.ViewFinishedLoading();
        }

        /// <summary>
        /// Notifies the view controller that its view is about to be added to a view hierarchy.
        /// </summary>
        /// <param name="animated">If true, the appearance of the view is being animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.AddNotificationObserver(
                NSNotificationCenter.DefaultCenter.AddObserver(
                    UIApplication.ContentSizeCategoryChangedNotification,
                    _ => this.HandleContentSizeCategoryChanged()));

            this.RegisterNotificationHooks();

            if (this.ensureNavigationBarVisible)
            {
                this.SetNavigationBarHidden(false, animated);
            }

            if (this.ensureNavigationBarHidden)
            {
                this.SetNavigationBarHidden(true, animated);
            }
        }

        /// <summary>
        /// Occurs when the view is about to be removed from the view hierarchy.
        /// </summary>
        /// <param name="animated">If true, the disappearance of the view is being animated.</param>
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            this.RemoveNotificationObservers();
        }

        /// <summary>
        /// Binds the user interface to the view model.
        /// </summary>
        protected abstract void Bind();

        /// <summary>
        /// Gets a loading overlay to show when awaiting for any background loading tasks to complete when the view is first loaded.
        /// </summary>
        /// <returns>A loading overlay.</returns>
        protected abstract ILoadingOverlay GetBackgroundLoadingOverlay();

        /// <summary>
        /// Handles the view did load event as an async operation.
        /// </summary>
        /// <returns>The task.</returns>
        protected virtual async Task HandleViewDidLoad()
        {
            if (!this.IsChild)
            {
                this.InitializeNavigationBar();
            }

            if (this.ViewModel.IsLoading)
            {
                UIView transitionView = this.GetTransitionView(this.View.Frame);

                this.View.AddSubview(transitionView);
                this.View.BringSubviewToFront(transitionView);

                await this.ViewModel.AwaitBackgroundLoading(this.GetBackgroundLoadingOverlay());

                transitionView.RemoveFromSuperview();

                this.InitializeViewAndBind();
            }
            else
            {
                this.InitializeViewAndBind();
            }
        }

        /// <summary>
        /// Initializes and updates any visual appearances for the views owned by this controller.
        /// </summary>
        protected virtual void InitializeViews()
        {
            this.StartIntroAnimations();
        }

        /// <summary>
        /// Starts any animations that are meant to be played when the view is first shown to the user.
        /// </summary>
        protected virtual void StartIntroAnimations()
        {
        }

        /// <summary>
        /// The view controller should register any notification hooks or events.
        /// </summary>
        protected virtual void RegisterNotificationHooks()
        {
        }

        /// <summary>
        /// Gets a view to display while transitioning from a new screen into the current screen
        /// and a background loading task is still running. The current view does not get
        /// binded and initialized until after the loading task so the transition view gets
        /// shown while the user waits.
        /// </summary>
        /// <param name="frame">The frame for the view.</param>
        /// <returns>A transition view.</returns>
        /// <remarks>
        /// This is different from the loading overlay in that the loading overlay is intended
        /// to cover the entire screen and is not shown during the transition between views.
        /// Whereas the transition view is added as a subview to the current view and is intended
        /// to cover any pre-initialized views until after the background loading has finished.
        /// </remarks>
        protected virtual UIView GetTransitionView(CGRect frame)
        {
            UIView view = new UIView(frame);
            view.BackgroundColor = UIColor.White;
            return view;
        }

        /// <summary>
        /// Creates a new view controller that is intended to represent a child view to be shown inside the current view controller.
        /// A child view controller will be allowed to or modify decorator type views such as the navigation bar or status bar.
        /// </summary>
        /// <param name="parameter">An optional parameter that is passed to the view model.</param>
        /// <returns>A new view controller.</returns>
        protected UIViewController<TChildViewModel> CreateChildViewController<TChildViewModel>(object parameter)
            where TChildViewModel : ViewModel
        {
            UIViewController<TChildViewModel> child = this.viewControllerFactory.InstantiateViewController<TChildViewModel>(parameter);
            child.IsChild = true;
            return child;
        }

        /// <summary>
        /// Registers a scroll view with the current controller to handle adjusting the view
        /// when a keyboard is shown to prevent edit fields from being hidden by the keyboard.
        /// </summary>
        /// <param name="scrollView">A scroll view.</param>
        protected void RegisterKeyboardHooksForScrollView(UIScrollView scrollView)
        {
            if (this.scrollView != null)
            {
                throw new InvalidOperationException("A scroll view has already been registered.");
            }

            // TODO: what if keyboard is already shown?
            this.scrollView = scrollView;

            this.RegisterKeyboardDidShow(e =>
            {
                this.originalContentInsets = this.scrollView.ContentInset;

                NSValue keyboardFrame = (NSValue)e.Notification.UserInfo.ObjectForKey(UIKeyboard.FrameBeginUserInfoKey);
                nfloat keyboardHeight = keyboardFrame.CGRectValue.Height;
                UIEdgeInsets contentInsets = new UIEdgeInsets(this.originalContentInsets.Value.Top, 0, keyboardHeight, 0);

                this.scrollView.ContentInset = contentInsets;
                this.scrollView.ScrollIndicatorInsets = contentInsets;

                UIView firstResponder = this.FindFirstResponder(this.scrollView);
                if (firstResponder != null)
                {
                    CGRect visibleRect =
                        new CGRect(
                            this.View.Frame.X,
                            this.View.Frame.Y,
                            this.View.Frame.Width,
                            this.View.Frame.Height - keyboardHeight);

                    CGPoint locationInScrollView = this.scrollView.ConvertPointFromView(firstResponder.Frame.Location, firstResponder);
                    if (!visibleRect.Contains(locationInScrollView))
                    {
                        this.scrollView.ScrollRectToVisible(new CGRect(locationInScrollView, firstResponder.Frame.Size), false);
                    }
                }
            });

            this.RegisterKeyboardWillBeHidden(e =>
            {
                // If the current view controller does not have the keyboard visible and then pushes another
                // view controller onto the stack, this delegate (on the parent) will get invoked if the child 
                // view controller has the keyboard visible when navigating back to the parent view controller.

                if (this.originalContentInsets.HasValue)
                {
                    this.scrollView.ContentInset = this.originalContentInsets.Value;
                    this.scrollView.ScrollIndicatorInsets = this.originalContentInsets.Value;
                }

                this.originalContentInsets = null;
            });
        }

        /// <summary>
        /// Handles when the dynamic type for the current view has changed. This occurs when the
        /// user changes the content size in the device's settings while the app is in the background. 
        /// This won't get invoked until the user brings it back into the foreground.
        /// </summary>
        protected virtual void HandleContentSizeCategoryChanged()
        {
        }

        /// <summary>
        /// Occurs when the controller is being disposed.
        /// </summary>
        /// <param name="disposing">True if the controller was explicilty disposed.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.RemoveNotificationObservers();
        }

        /// <summary>
        /// Registers a delegate to listen for when the application has become active from the background.
        /// </summary>
        /// <param name="becameActive">Gets invoked when the application has become active.</param>
        protected void RegisterDidBecomeActive(Action becameActive)
        {
            this.AddNotificationObserver(
                NSNotificationCenter.DefaultCenter.AddObserver(
                    UIApplication.DidBecomeActiveNotification,
                    _ => becameActive()));
        }

        /// <summary>
        /// Indicates that the navigation bar for the current view should be visible whenever the view is shown to the user.
        /// </summary>
        protected void EnsureNavigationBarVisible()
        {
            if (this.IsChild)
            {
                throw new InvalidOperationException("The current view controller is a child.");
            }

            this.ensureNavigationBarVisible = true;
        }

        /// <summary>
        /// Indicates that the navigation bar for the current view should be hidden whenever the view is shown to the user.
        /// </summary>
        protected void EnsureNavigationBarHidden()
        {
            if (this.IsChild)
            {
                throw new InvalidOperationException("The current view controller is a child.");
            }

            this.ensureNavigationBarHidden = true;
        }

        /// <summary>
        /// Registers a series of input fields to form a 'chain' as the user moves from field-to-field.
        /// There is a little bit of work that needs to be setup in the controller's storyboard and
        /// this logic will handle moving from the first field in the chain to the next field when the user
        /// hits the Next (Return) button on the keyboard and the last field in the chain will instead trigger
        /// the supplied Action.
        /// </summary>
        /// <param name="action">The action to invoke when the user hits the Go (Return) button while in the last field.</param>
        /// <param name="textFields">A list of fields used to form a chain.</param>
        protected void RegisterInputChain(Action action, params UITextField[] textFields)
        {
            if (textFields.Length < 1)
            {
                throw new ArgumentException("You must specify at least 1 field in order to form a chain.", nameof(textFields));
            }

            if (textFields.Length > 1)
            {
                for (int i = 0; i < textFields.Length - 1; i++)
                {
                    this.RegisterNextField(textFields[i], textFields[i + 1]);
                }
            }

            UITextField lastField = textFields.Last();
            this.RegisterGo(lastField, action);
        }

        /// <summary>
        /// Registers a delegate with the specified focused field that will handle moving the
        /// input focus to the next field when the Next (Return) button on the keyboard is pressed.
        /// </summary>
        /// <param name="focusField">The field that is expected to have input focus.</param>
        /// <param name="nextField">The field to give the input focus to.</param>
        protected void RegisterNextField(UITextField focusField, UITextField nextField)
        {
            focusField.ReturnKeyType = UIReturnKeyType.Next;
            focusField.ShouldReturn += field =>
            {
                nextField.BecomeFirstResponder();
                return true;
            };
        }

        /// <summary>
        /// Registers a delegate with the specified focused field that will invoke the
        /// given action when the Go (Return) button on the keyboard is pressed.
        /// </summary>
        /// <param name="focusField">The field that is expected to have input focus.</param>
        /// <param name="action">The action to invoke.</param>
        protected void RegisterGo(UITextField focusField, Action action)
        {
            focusField.ReturnKeyType = UIReturnKeyType.Go;
            focusField.ShouldReturn += field =>
            {
                action();
                return true;
            };
        }

        /// <summary>
        /// Creates a view for the navigation bar that displays the app's logo.
        /// </summary>
        /// <returns>A new UIView.</returns>
        protected virtual UIView CreateNavigationLogoView()
        {
            return new UIView();
        }

        /// <summary>
        /// Updates the navigation title based on the current controller's view model.
        /// </summary>
        protected void UpdateNavigationTitle()
        {
            NavigationBarTitle title = this.ViewModel.GetNavigationBarTitle();
            if (title.TitleType == NavigationBarTitle.NavigationBarTitleType.Logo)
            {
                this.NavigationItem.TitleView = this.CreateNavigationLogoView();
            }
            else
            {
                this.NavigationItem.Title = title.Text ?? string.Empty;
            }
        }

        /// <summary>
        /// Initializes the navigation bar.
        /// </summary>
        internal protected virtual void InitializeNavigationBar()
        {
            this.UpdateNavigationTitle();
            this.InitializeNavigationBackButton();
        }

        /// <summary>
        /// Initializes this controller and sets the view model.
        /// </summary>
        /// <param name="viewControllerFactory">The view controller factory for the app.</param>
        /// <param name="viewModel">View model for the controller.</param>
        internal void Initialize(UIViewControllerFactory viewControllerFactory, TViewModel viewModel)
        {
            if (this.ViewModel != null)
            {
                throw new InvalidOperationException("The view controller has already been initialized.");
            }

            this.ViewModel = viewModel;
            this.viewControllerFactory = viewControllerFactory;
        }

        /// <summary>
        /// Registers a delegate to listen for the keyboard will be shown event.
        /// </summary>
        /// <param name="keyboardShown">Gets invoked when the keyboard is about to be shown.</param>
        internal void RegisterKeyboardWillBeShown(Action<UIKeyboardEventArgs> keyboardShown)
        {
            this.AddNotificationObserver(
                NSNotificationCenter.DefaultCenter.AddObserver(
                    UIKeyboard.WillShowNotification,
                    notification => keyboardShown(new UIKeyboardEventArgs(notification))));
        }

        /// <summary>
        /// Registers a delegate to listen for the keyboard did show event.
        /// </summary>
        /// <param name="keyboardShown">Gets invoked when the keyboard has been shown.</param>
        internal void RegisterKeyboardDidShow(Action<UIKeyboardEventArgs> keyboardShown)
        {
            this.AddNotificationObserver(
                NSNotificationCenter.DefaultCenter.AddObserver(
                    UIKeyboard.DidShowNotification,
                    notification => keyboardShown(new UIKeyboardEventArgs(notification))));
        }

        /// <summary>
        /// Registers a delegate to listen for the keyboard will be hidden event.
        /// </summary>
        /// <param name="keyboardHidden">Gets invoked when the keyboard is about to be hidden.</param>
        internal void RegisterKeyboardWillBeHidden(Action<UIKeyboardEventArgs> keyboardHidden)
        {
            this.AddNotificationObserver(
                NSNotificationCenter.DefaultCenter.AddObserver(
                    UIKeyboard.WillHideNotification,
                    notification => keyboardHidden(new UIKeyboardEventArgs(notification))));
        }

        private void InitializeViewAndBind()
        {
            this.InitializeViews();
            this.Bind();
        }

        private void InitializeNavigationBackButton()
        {
            if (this.NavigationItem.BackBarButtonItem == null)
            {
                // remove text from the back button, by default it would be the text of the previos screen's title
                this.NavigationItem.BackBarButtonItem = new UIBarButtonItem(string.Empty, UIBarButtonItemStyle.Plain, null);
            }
        }

        private void SetNavigationBarHidden(bool hide, bool animated)
        {
            this.NavigationController.SetNavigationBarHidden(hide, animated);
        }

        private void AddNotificationObserver(NSObject observer)
        {
            if (this.notificationObservers == null)
            {
                this.notificationObservers = new List<NSObject>();
            }

            this.notificationObservers.Add(observer);
        }

        private void RemoveNotificationObservers()
        {
            if (this.notificationObservers != null)
            {
                foreach (NSObject observer in this.notificationObservers)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
                }

                this.notificationObservers.Clear();
                this.scrollView = null;
            }
        }

        private UIView FindFirstResponder(UIView superview)
        {
            UIView firstResponder = null;
            foreach (UIView view in superview.Subviews)
            {
                firstResponder =
                    view.IsFirstResponder
                    ? view
                    : this.FindFirstResponder(view);

                if (firstResponder != null)
                {
                    break;
                }
            }

            return firstResponder;
        }
    }
}