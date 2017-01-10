using System;
using Mitten.Mobile.Application;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Defines an overlay to display when a long running background operation is taking place.
    /// </summary>
    public interface ILoadingOverlay
    {
        /// <summary>
        /// Shows a loading overlay on top of the current screen.
        /// </summary>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <returns>An object that will be disposed when the overlay should be hidden and destroyed.</returns>
        IDisposable ShowLoadingOverlay(IProgress progress = null);
    }
}