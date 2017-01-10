using System;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Represents an error due to an invalid view model operation performed prior to finishing its background loading.
    /// </summary>
    public class ViewModelNotLoadedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelNotLoadedException class.
        /// </summary>
        internal ViewModelNotLoadedException()
            : base("The view model has not completed background loading.")
        {
        }
    }
}