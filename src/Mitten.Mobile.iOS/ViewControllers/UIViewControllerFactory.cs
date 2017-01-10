using System;
using System.Linq;
using System.Reflection;
using Foundation;
using Mitten.Mobile.Application;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Factory class for creating view controllers based on a specific view model.
    /// </summary>
    internal class UIViewControllerFactory
    {
        private readonly ViewControllerTypes viewControllerTypes;
        private readonly ApplicationHost applicationHost;

        /// <summary>
        /// Initializes a new instance of the UIViewControllerFactory class.
        /// </summary>
        /// <param name="applicationHost">The host for the current application.</param>
        /// <param name="viewControllerTypes">A list of known view controller types and their associated view models.</param>
        public UIViewControllerFactory(ApplicationHost applicationHost, ViewControllerTypes viewControllerTypes)
        {
            Throw.IfArgumentNull(applicationHost, nameof(applicationHost));
            Throw.IfArgumentNull(viewControllerTypes, nameof(viewControllerTypes));

            this.applicationHost = applicationHost;
            this.viewControllerTypes = viewControllerTypes;
        }

        /// <summary>
        /// Instantiates a new instance of a view controller.
        /// </summary>
        /// <param name="parameter">An optional parameter that is passed to the view model.</param>
        /// <returns>A new view controller.</returns>
        /// <remarks>
        /// This is a little complex due to a circular dependency between the view model, navigation, and view controller.
        /// (VM -- depends on --> Navigation -- depends on --> VC -- depends on --> VM).
        /// </remarks>
        public UIViewController<TViewModel> InstantiateViewController<TViewModel>(object parameter)
            where TViewModel : ViewModel
        {
            Type viewControllerType = this.viewControllerTypes.GetViewControllerType<TViewModel>();
            UIStoryboard storyboard = this.GetStoryboard(viewControllerType);

            UIViewController<TViewModel> viewController = 
                storyboard != null
                ? this.InstantiateViewController<TViewModel>(viewControllerType, storyboard)
                : this.InstantiateViewController<TViewModel>(viewControllerType);

            TViewModel viewModel = ViewModelFactory.Create<TViewModel>(this.applicationHost, new Navigation(this, viewController), parameter);
            viewController.Initialize(this, viewModel);

            return viewController;
        }

        private UIViewController<TViewModel> InstantiateViewController<TViewModel>(Type viewControllerType)
            where TViewModel : ViewModel
        {
            UIViewController<TViewModel> viewController = Activator.CreateInstance(viewControllerType) as UIViewController<TViewModel>;

            if (viewController == null)
            {
                throw new InvalidOperationException("The UIViewController of Type " + viewControllerType.FullName + " must inhert the UIViewController<TViewModel> abstract class.");
            }

            return viewController;
        }

        private UIViewController<TViewModel> InstantiateViewController<TViewModel>(Type viewControllerType, UIStoryboard storyboard)
            where TViewModel : ViewModel
        {
            try
            {
                UIViewController<TViewModel> viewController = storyboard.InstantiateViewController(viewControllerType.Name) as UIViewController<TViewModel>;

                if (viewController == null)
                {
                    throw new InvalidOperationException("The UIViewController of Type " + viewControllerType.FullName + " must inhert the UIViewController<TViewModel> abstract class.");
                }

                return viewController;
            }
            catch (MonoTouchException ex)
            {
                if (ex.NSException.Name == "NSInvalidArgumentException")
                {
                    throw new ArgumentException(
                        "The storyboard " + this.GetStoryboardName(viewControllerType) + " does not have a view controller with Restoration ID of " + viewControllerType.Name + ". " +
                        "Make sure you have the right storyboard and that the view controller in the storyboard has the same Restoration ID as the class name.",
                        ex);
                }

                throw;
            }
        }

        private string GetStoryboardName(Type viewControllerType)
        {
            return this.GetStoryboardAttribute(viewControllerType).StoryboardName;
        }

        private UIStoryboard GetStoryboard(Type viewControllerType)
        {
            StoryboardAttribute attribute = this.GetStoryboardAttribute(viewControllerType);

            return 
                attribute != null 
                ? attribute.GetStoryboard() 
                : null;
        }

        private StoryboardAttribute GetStoryboardAttribute(Type viewControllerType)
        {
            return viewControllerType.GetCustomAttributes(typeof(StoryboardAttribute)).SingleOrDefault() as StoryboardAttribute;
        }
    }
}