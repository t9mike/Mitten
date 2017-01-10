using System;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Represents when a UI view controller cannot be found.
    /// </summary>
    public class ViewControllerNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ViewControllerNotFoundException class.
        /// </summary>
        /// <param name="viewModelType">A view model Type for which a matching view controller could not be found.</param>
        internal ViewControllerNotFoundException(Type viewModelType)
            : base("Unable to find view controller for view model (" + viewModelType.Name + ").")
        {
        }
    }
}