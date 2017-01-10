namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Identifies how a view should be presented when navigating between views.
    /// </summary>
    public enum PresentationType
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Represents the platforms standard approach for navigating between views.
        /// </summary>
        Standard,

        /// <summary>
        /// Represents a modal view, meaning the view should be displayed on top of any existing view,
        /// support navigating between additional screens, and displayed full screen by hiding any top 
        /// navigation/title bars.
        /// </summary>
        ModalWithNavigationFullScreen,

        /// <summary>
        /// Represents a modal view, meaning the view should be displayed on top of any existing view,
        /// support navigating between additional screens, and displayed with a top navigation/title bar.
        /// </summary>
        ModalWithNavigation,

        /// <summary>
        /// Represents that a view should be made the parent view for the application.
        /// </summary>
        Root,
    }
}