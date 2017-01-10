using System;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Contains options for a navigation action.
    /// </summary>
    public class NavigationOptions
    {
        /// <summary>
        /// The default options used for navigation.
        /// </summary>
        public static readonly NavigationOptions Default = new NavigationOptions();

        private readonly Action completionHandler;

        /// <summary>
        /// Initializes a new instance of the NavigationOptions class.
        /// </summary>
        /// <param name="parameter">A parameter to pass to the view model being navigated to.</param>
        /// <param name="presentationType">Identifies how the screen should be presented to the user.</param>
        /// <param name="animateTransitionToNextView">True if the transition to the next screen should be animated, the default is true.</param>
        /// <param name="completionHandler">An optional callback that will be invoked upon the completion of the navigation transition.</param>
        public NavigationOptions(
            object parameter = null, 
            PresentationType presentationType = PresentationType.Standard,
            bool animateTransitionToNextView = true,
            Action completionHandler = null)
        {
            if (presentationType == PresentationType.Invalid)
            {
                throw new ArgumentException("PresentationType cannot be 'Invalid'.");
            }

            this.Parameter = parameter;
            this.PresentationType = presentationType;
            this.AnimateTransitionToNextView = animateTransitionToNextView;
            this.completionHandler = completionHandler;
        }

        /// <summary>
        /// Gets whether or not the transition to the next view should be animated.
        /// </summary>
        public bool AnimateTransitionToNextView { get; private set; }

        /// <summary>
        /// Gets the presentation type which is used to identify how the new view should be presented.
        /// </summary>
        public PresentationType PresentationType { get; private set; }

        /// <summary>
        /// Gets an optional parameter for the view model being navigated to.
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Signals that the navigation has completed.
        /// </summary>
        public void SignalNavigationComplete()
        {
            if (this.completionHandler != null)
            {
                this.completionHandler();
            }
        }
    }
}