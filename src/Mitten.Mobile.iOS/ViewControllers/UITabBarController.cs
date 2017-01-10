using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Base class for a view controller that host multiple child controllers inside a tab bar layout.
    /// </summary>
    public abstract class UITabBarController<TViewModel> : UIViewController<TViewModel>
        where TViewModel : ViewModel
    {
        private readonly Dictionary<nint, ChildViewController> childViewControllers;

        private UIViewController currentChildController;
        private Task viewLoadTask;
        private nint currentItemTag;

        /// <summary>
        /// Initializes a new instance of the UITabBarController class.
        /// </summary>
        protected UITabBarController()
        {
            this.childViewControllers = new Dictionary<nint, ChildViewController>();
        }

        /// <summary>
        /// Initializes a new instance of the UITabBarController class.
        /// </summary>
        /// <param name="handle">Object handle used by the runtime.</param>
        protected UITabBarController(IntPtr handle)
            : base(handle)
        {
            this.childViewControllers = new Dictionary<nint, ChildViewController>();
        }

        /// <summary>
        /// Gets the tab bar for this controller.
        /// </summary>
        protected abstract UITabBar TabBar { get; }

        /// <summary>
        /// Gets an optional UIView that is used as a placeholder for the children views.
        /// </summary>
        protected abstract UIView ChildPlaceholderView { get; }

        /// <summary>
        /// Gets or sets whether or not the navigation bar should be updated based on the
        /// NavigationItem of the child view controller, the default is false.
        /// </summary>
        protected bool UseChildNavigationItem { get; set; }

        /// <summary>
        /// Occurs when the view controller that its view was added to a view hierarchy.
        /// </summary>
        /// <param name="animated">If true, the view was added to the window using an animation.</param>
        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            await this.viewLoadTask;

            if (this.TabBar.SelectedItem == null)
            {
                this.OnViewInitialized(tabToShow =>
                {
                    UITabBarItem itemToShow = this.TabBar.Items.SingleOrDefault(item => item.Tag == tabToShow);
                    if (itemToShow == null)
                    {
                        throw new ArgumentException("No tab bar item exits with tag (" + tabToShow + ").", nameof(tabToShow));
                    }

                    this.TabBar.SelectedItem = itemToShow;
                    this.ShowTab(itemToShow.Tag);
                });
            }
        }

        /// <summary>
        /// Occurs when the subviews for the view have been laid out.
        /// </summary>
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (this.ChildPlaceholderView != null &&
                this.currentChildController != null)
            {
                this.currentChildController.View.Frame = this.ChildPlaceholderView.Frame;
            }
        }

        /// <summary>
        /// Handles the view did load event as an async operation.
        /// </summary>
        /// <returns>The task.</returns>
        protected override Task HandleViewDidLoad()
        {
            this.viewLoadTask = base.HandleViewDidLoad();
            return this.viewLoadTask;
        }

        /// <summary>
        /// Initializes and updates any visual appearances for the views owned by this controller.
        /// </summary>
        protected override void InitializeViews()
        {
            base.InitializeViews();
            this.TabBar.ItemSelected += (sender, e) => this.ShowTab(e.Item.Tag);
        }

        /// <summary>
        /// Indicates that the view has been initialized and requests the id for the tab to show.
        /// </summary>
        /// <param name="showTab">Shows the tab with the specified id.</param>
        protected abstract void OnViewInitialized(Action<int> showTab);

        /// <summary>
        /// Registers a tag with the specified view model type.
        /// </summary>
        /// <param name="tag">A tag identifying a tab bar item.</param>
        protected void RegisterChildViewModel<TChildViewModel>(nint tag)
            where TChildViewModel : ViewModel
        {
            this.RegisterChildViewModel<TChildViewModel>(tag, () => null);
        }

        /// <summary>
        /// Registers a tag with the specified view model type.
        /// </summary>
        /// <param name="tag">A tag identifying a tab bar item.</param>
        /// <param name="getParameter">Gets a parameter to pass to the view model when it is instantiated.</param>
        protected void RegisterChildViewModel<TChildViewModel>(nint tag, Func<object> getParameter)
            where TChildViewModel : ViewModel
        {
            this.EnsureTabBarItemExists(tag);

            if (this.childViewControllers.ContainsKey(tag))
            {
                throw new ArgumentException("A child view model is already registered. If you need to register multiple view models, use the RegisterDynamicChildViewModel method instead.");
            }

            this.childViewControllers.Add(
                tag,
                new ChildViewController(
                    () => this.CreateChildViewController<TChildViewModel>(getParameter()),
                    viewController => ((UIViewController<TChildViewModel>)viewController).InitializeNavigationBar()));
        }

        /// <summary>
        /// Shows the tab with the specified tag.
        /// </summary>
        /// <param name="tag">The tag assigned to the tab to show.</param>
        protected void ShowTab(nint tag)
        {
            if (tag != this.currentItemTag)
            {
                ChildViewController childController = this.LoadChildViewController(tag);

                if (this.currentChildController != null)
                {
                    this.currentChildController.ViewWillDisappear(false);
                    this.currentChildController.View.RemoveFromSuperview();
                    this.currentChildController.RemoveFromParentViewController();
                }

                this.AddChildViewController(childController.Instance);
                childController.Instance.DidMoveToParentViewController(this);

                childController.Instance.ViewWillAppear(false);

                this.InsertAndLayoutTabView(childController.Instance);

                if (this.UseChildNavigationItem)
                {
                    childController.InitializeNavigationBar();
                    this.SetNavigationItemFromChild(childController.Instance);
                }

                if (this.currentChildController != null)
                {
                    this.currentChildController.ViewDidDisappear(false);
                }

                childController.Instance.ViewDidAppear(false);

                this.currentChildController = childController.Instance;
                this.currentItemTag = tag;
            }
        }

        /// <summary>
        /// Lays out the view for the specified controller and inserts it into view.
        /// </summary>
        /// <param name="tabBarChildController">The view controller for the tab to show.</param>
        protected virtual void InsertAndLayoutTabView(UIViewController tabBarChildController)
        {
            if (this.ChildPlaceholderView == null)
            {
                // TODO: come up with a cleaner solution
                throw new InvalidOperationException("If a ChildPlaceholderView is not specified, this subclass must override the InsertAndLayoutTabView method.");
            }

            UIView superview = this.ChildPlaceholderView.Superview;
            tabBarChildController.View.Frame = this.ChildPlaceholderView.Frame;
            superview.InsertSubviewAbove(tabBarChildController.View, this.ChildPlaceholderView);
        }

        private void SetNavigationItemFromChild(UIViewController tabBarChildController)
        {
            this.NavigationItem.LeftBarButtonItems = tabBarChildController.NavigationItem.LeftBarButtonItems;
            this.NavigationItem.RightBarButtonItems = tabBarChildController.NavigationItem.RightBarButtonItems;
        }

        private void EnsureTabBarItemExists(nint tag)
        {
            if (!this.TabBar.Items.Any(item => item.Tag == tag))
            {
                throw new ArgumentException("No tab bar item found with tag (" + tag + ").", nameof(tag));
            }
        }

        private ChildViewController LoadChildViewController(nint tag)
        {
            ChildViewController childViewController;
            if (!this.childViewControllers.TryGetValue(tag, out childViewController))
            {
                throw new ArgumentException("No child view controller has been registered for tab bar with item tag (" + tag + ").", nameof(tag));
            }

            if (childViewController.Instance == null)
            {
                childViewController.CreateViewController();
            }

            return childViewController;
        }

        private class ChildViewController
        {
            private readonly Func<UIViewController> createViewController;
            private readonly Action<UIViewController> initializeNavigationBar;

            public ChildViewController(
                Func<UIViewController> createViewController,
                Action<UIViewController> initializeNavigationBar)
            {
                this.createViewController = createViewController;
                this.initializeNavigationBar = initializeNavigationBar;
            }

            public UIViewController Instance { get; private set; }

            public void InitializeNavigationBar()
            {
                if (this.Instance == null)
                {
                    throw new InvalidOperationException("The view controller has not been created.");
                }

                this.initializeNavigationBar(this.Instance);
            }

            public void CreateViewController()
            {
                this.Instance = this.createViewController();
            }
        }
    }
}
