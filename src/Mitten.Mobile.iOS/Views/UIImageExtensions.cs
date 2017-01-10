using System;
using CoreGraphics;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains extension methods for a UIImage.
    /// </summary>
    public static class UIImageExtensions
    {
        /// <summary>
        /// Decompresses the specified UIImage.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <returns>The decompressed image.</returns>
        public static UIImage Decompress(this UIImage image)
        {
            UIGraphics.BeginImageContext(image.Size);
            image.Draw(CGPoint.Empty);
            UIImage decompressedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return decompressedImage;
        }

        /// <summary>
        /// Gets an image from the current image with the template rendering mode.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <returns>An image.</returns>
        public static UIImage AsTemplate(this UIImage image)
        {
            return image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        /// <summary>
        /// Adds a transparent padding around all edges of an image.
        /// </summary>
        /// <param name="image">The image to update.</param>
        /// <param name="padding">The amount of padding to add.</param>
        /// <returns>A new image containing the transparent padding.</returns>
        public static UIImage WithPadding(this UIImage image, nfloat padding)
        {
            nfloat newWidth = image.Size.Width + (padding * UIScreen.MainScreen.Scale);
            nfloat newHeight = image.Size.Height + (padding * UIScreen.MainScreen.Scale);

            UIGraphics.BeginImageContextWithOptions(new CGSize(newWidth, newHeight), false, 0);
            CGContext context = UIGraphics.GetCurrentContext();

            UIGraphics.PushContext(context);
            image.Draw(new CGPoint(padding, padding));
            UIGraphics.PopContext();

            UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return newImage;
        }
    }
}