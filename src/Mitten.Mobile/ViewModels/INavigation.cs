namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Defines a generic platform-independant interface for UI navigation.
    /// </summary>
    public interface INavigation
    {
        /// <summary>
        /// Navigates to a view specified by the view model type.
        /// </summary>
        /// <param name="parameter">An optional parameter to pass to the view model being navigated to.</param>
        void NavigateTo<TViewModel>(object parameter = null) 
            where TViewModel : ViewModel;

        /// <summary>
        /// Navigates to a view specified by the view model type.
        /// </summary>
        /// <param name="options">Additional options for the navigation request.</param>
        void NavigateTo<TViewModel>(NavigationOptions options) 
            where TViewModel : ViewModel;
        
        /// <summary>
        /// Navigates back to the previous screen.
        /// </summary>
        void Back();

        /// <summary>
        /// Navigates back to the root view.
        /// </summary>
        void BackToRoot();

        /// <summary>
        /// Closes the current view if it, or its parent, was shown modally.
        /// </summary>
        void Close();
    }
}