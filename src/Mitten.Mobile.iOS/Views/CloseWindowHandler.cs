using System;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Handles closing a specified window upon being disposed.
    /// </summary>
    public class CloseWindowHandler : IDisposable
    {
        private UIWindow window;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the CloseWindowHandler class.
        /// </summary>
        /// <param name="window">A window.</param>
        public CloseWindowHandler(UIWindow window)
        {
            this.window = window;
        }

        /// <summary>
        /// Disposes the current helper and closes the window.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.window.Hidden = true;
                this.window.Dispose();
                this.window = null;

                this.isDisposed = true;
            }
        }
    }
}
