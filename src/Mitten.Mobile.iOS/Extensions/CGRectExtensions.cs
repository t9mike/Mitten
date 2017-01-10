using System;
using CoreGraphics;

namespace Finch.Mobile.iOS.Extensions
{
    /// <summary>
    /// Contains extensions for the CGRect class.
    /// </summary>
    public static class CGRectExtensions
    {
        /// <summary>
        /// Gets the center point for the current rectangle.
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <returns>The center point.</returns>
        public static CGPoint GetCenter(this CGRect rect)
        {
            return new CGPoint(rect.GetMidX(), rect.GetMidY());
        }

        /// <summary>
        /// Gets a new CGRect with the specified x-coordinate.
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="x">The x-coordinate for the rectangle.</param>
        /// <returns>A new CGRect with the specified x-coordinate.</returns>
        public static CGRect WithX(this CGRect rect, nfloat x)
        {
            return new CGRect(x, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets a new CGRect with the specified y-coordinate.
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="y">The y-coordinate for the rectangle.</param>
        /// <returns>A new CGRect with the specified y-coordinate.</returns>
        public static CGRect WithY(this CGRect rect, nfloat y)
        {
            return new CGRect(rect.X, y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets a new CGRect with the specified height.
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="height">The height for the rectangle.</param>
        /// <returns>A new CGRect with the specified height.</returns>
        public static CGRect WithHeight(this CGRect rect, nfloat height)
        {
            return new CGRect(rect.X, rect.Y, rect.Width, height);
        }

        /// <summary>
        /// Gets a new CGRect with the specified width.
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="width">The width for the rectangle.</param>
        /// <returns>A new CGRect with the specified width.</returns>
        public static CGRect WithWidth(this CGRect rect, nfloat width)
        {
            return new CGRect(rect.X, rect.Y, width, rect.Height);
        }
    }
}