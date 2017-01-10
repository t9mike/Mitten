using System;
using CoreGraphics;
using Mitten.Mobile.Graphics;
using UIKit;

namespace Mitten.Mobile.iOS.Extensions
{
    /// <summary>
    /// Contains extensions for the Color struct.
    /// </summary>
    public static class ColorExtensions
    {
        private static class Constants
        {
            public static readonly nfloat MaxByteValue = byte.MaxValue;
        }

        /// <summary>
        /// Gets a CGColor object based on the current color.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A new CGColor.</returns>
        public static CGColor ToCGColor(this Color color)
        {
            return 
                new CGColor(
                    color.Red / Constants.MaxByteValue,
                    color.Green / Constants.MaxByteValue,
                    color.Blue / Constants.MaxByteValue,
                    color.Alpha / Constants.MaxByteValue);
        }

        /// <summary>
        /// Gets a UIColor object based on the current color.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A new UIColor.</returns>
        public static UIColor ToUIColor(this Color color)
        {
            return 
                new UIColor(
                    color.Red / Constants.MaxByteValue,
                    color.Green / Constants.MaxByteValue,
                    color.Blue / Constants.MaxByteValue,
                    color.Alpha / Constants.MaxByteValue);
        }
    }
}