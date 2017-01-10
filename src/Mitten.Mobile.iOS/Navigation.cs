using System;
using Mitten.Mobile.iOS.ViewControllers;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS
{
    /// <summary>
    /// Handles iOS navigation between screens.
    /// </summary>
    /// <remarks>
    /// There are 2 concepts in an iOS navigation that we will be working with:
    /// 
    /// Hierarchical:
    /// Navigation flows forward and backward through screens creating a natural flow
    /// for the user to collect/navigate data. Hierarchical navigation is managed by the
    /// UINavigationViewController by Pushing/Popping controllers on and off its internal
    /// stack. In iOS, you commonly see this when the screen slides left or right.
    /// 
    /// Modal:
    /// A screen that is shown modally is shown on top of the current screen covering it 
    /// completely. You can think of a modal view almost like its own window and it does not
    /// share the UINavigationController of its Presenting controller. In iOS, a controller
    /// Presents another controller modally and is known as the Presenting controller
    /// whereas the controller being shown modally is known as the Presented controller.
    /// A controller that is shown modally must be Dismissed since it is not 'Pushed' 
    /// onto any navigation controllers.
    /// 
    /// Notes:
    /// We will be working under the assumption that all controllers will be 
    /// UIViewControllers and will not subclass the UINavigationController. It is common
    /// to display Hierarchical navigation in a modal view. To do this, the Presented
    /// controller will get wrapped inside a new instance of a UINavigationController.
    /// </remarks>
    internal class Navigation : INavigation
    {
        private static class Constants
        {
            public const double TransitionDuration = 0.3;
        }

        private readonly UIViewControllerFactory viewControllerFactory;
        private readonly UIViewController currentController;

        /// <summary>
        /// Initializes a new instance of the Navigation class.
        /// </summary>
        /// <param name="viewControllerFactory">A view controller factory.</param>
        /// <param name="currentController">The controller for the currently visible view that will be making the navigation requests.</param>
        public Navigation(UIViewControllerFactory viewControllerFactory, UIViewController currentController = null)
        {
            Throw.IfArgumentNull(viewControllerFactory, nameof(viewControllerFactory));

            this.viewControllerFactory = viewControllerFactory;
            this.currentController = currentController;
        }

        /// <summary>
        /// Navigates to a view specified by the view model type.
        /// </summary>
        /// <param name="parameter">An optional parameter to pass to the view model being navigated to.</param>
        public void NavigateTo<TViewModel>(object parameter = null)
            where TViewModel : ViewModel
        {
            this.NavigateTo<TViewModel>(new NavigationOptions(parameter));
        }

        /// <summary>
        /// Navigates to a view specified by the view model type.
        /// </summary>
        /// <param name="options">Additional options for the navigation request.</param>
        public void NavigateTo<TViewModel>(NavigationOptions options)
            where TViewModel : ViewModel
        {
            if (options.PresentationType == PresentationType.ModalWithNavigation ||
                options.PresentationType == PresentationType.ModalWithNavigationFullScreen)
            {
                this.PresentWithNavigation<TViewModel>(
                    options, 
                    options.PresentationType == PresentationType.ModalWithNavigationFullScreen);
            }
            else if (options.PresentationType == PresentationType.Root)
            {
                // TODO: signal complete
                this.MakeRoot<TViewModel>(options);
            }
            else if (options.PresentationType == PresentationType.Standard)
            {
                // TODO: signal complete
                this.Push<TViewModel>(options);
            }
            else
            {
                throw new ArgumentException("Unknown PresentationType (" + options.PresentationType + ").");
            }
        }

        /// <summary>
        /// Navigates back to the previous screen.
        /// </summary>
        public void Back()
        {
            this.EnsureCurrentController();

            UINavigationController navigationController = this.currentController.NavigationController;
            if (navigationController == null)
            {
                throw new InvalidOperationException("The current view is not currently presented as part of a navigation controller.");
            }

            navigationController.PopViewController(true);
        }

        /// <summary>
        /// Navigates back to the root view.
        /// </summary>
        public void BackToRoot()
        {
            this.EnsureCurrentController();

            UINavigationController navigationController = this.currentController.NavigationController;
            if (navigationController == null)
            {
                throw new InvalidOperationException("The current view is not currently presented as part of a navigation controller.");
            }

            navigationController.PopToRootViewController(true);
        }

        /// <summary>
        /// Closes the current view if it, or its parent, was shown modally.
        /// </summary>
        public void Close()
        {
            this.EnsureCurrentController();

            if (this.currentController.PresentingViewController == null)
            {
                throw new InvalidOperationException("The current view is not currently presented as part of a modal controller.");
            }

            this.currentController.PresentingViewController.DismissViewController(true, null);
        }

        private void Push<TViewModel>(NavigationOptions options) 
            where TViewModel : ViewModel
        {
            this.EnsureCurrentController();

            UINavigationController navigationController = currentController.NavigationController;

            if (navigationController == null)
            {
                throw new InvalidOperationException(
                    "The current view is not currently presented as part of a navigation controller. " +
                    "If the view was originally shown modally, use the ShowModalWithNavigation method to enable forward navigation.");
            }

            UIViewController<TViewModel> viewControllerToPresent = this.viewControllerFactory.InstantiateViewController<TViewModel>(options.Parameter);
            navigationController.PushViewController(viewControllerToPresent, options.AnimateTransitionToNextView);
        }

        private void PresentWithNavigation<TViewModel>(NavigationOptions options, bool hideNavigationBar)
            where TViewModel : ViewModel
        {
            this.EnsureCurrentController();

            UIViewController viewControllerToPresent = this.viewControllerFactory.InstantiateViewController<TViewModel>(options.Parameter);
            UINavigationController navigationController = new UINavigationController(viewControllerToPresent);

            if (hideNavigationBar)
            {
                navigationController.NavigationBar.Hidden = true;
            }

            this.currentController.PresentViewController(navigationController, options.AnimateTransitionToNextView, options.SignalNavigationComplete);
        }

        private void EnsureCurrentController()
        {
            if (this.currentController == null)
            {
                throw new InvalidOperationException("The Navigation operation requires a UIViewController to be set.");
            }
        }

        private void MakeRoot<TViewModel>(NavigationOptions options)
            where TViewModel : ViewModel
        {
            if (this.currentController != null)
            {
                if (this.currentController.PresentingViewController != null)
                {
                    this.currentController.PresentingViewController.DismissViewController(false, null);
                }

                if (this.currentController.NavigationController != null)
                {
                    this.currentController.NavigationController.PopToRootViewController(false);
                }
            }

            UIViewController viewControllerToMakeRoot = this.viewControllerFactory.InstantiateViewController<TViewModel>(options.Parameter);
            UIWindow window = UIApplication.SharedApplication.KeyWindow;

            window.RootViewController = 
                viewControllerToMakeRoot.NavigationController == null
                ? new UINavigationController(viewControllerToMakeRoot)
                : viewControllerToMakeRoot;
            
            UIView.Transition(
                window,
                Constants.TransitionDuration, 
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => { },
                () => { });
        }
    }
}