using System;
using System.Drawing;
using CoreGraphics;
using Mitten.Mobile.Graphics;
using UIKit;

namespace Mitten.Mobile.iOS.Graphics
{
    /// <summary>
    /// Handles image transformations on the iOS platform.
    /// </summary>
    public class ImageTransform : IImageTransform
    {
        private static class Constants
        {
            public const float DefaultCompressionQuality = 0.5f;
        }

        private enum ScaleMode
        {
            Fit,
            Fill
        }

        private readonly float compressionQuality;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTransform"/> class.
        /// </summary>
        /// <param name="compressionQuality">
        /// The compression quality when scaling an image, the value ranging from 0.0 - 1.0 where 0.0 represents the maximum compression level.
        /// The default compress quality is defined in the application settings.
        /// </param>
        public ImageTransform(float compressionQuality = Constants.DefaultCompressionQuality)
        {
            this.compressionQuality = compressionQuality;
        }

        /// <summary>
        /// Scales an image to fill the specified bounds while maintaining aspect ratio.
        /// Any part of the image that spills outside the bounds will be cropped out.
        /// </summary>
        /// <param name="imageData">An array containing the image data.</param>
        /// <param name="width">The width, as the number of pixels, to for the bounds.</param>
        /// <param name="height">The height, as the number of pixels, to for the bounds.</param>
        /// <returns>A scaled image.</returns>
        public byte[] ScaleToFill(byte[] imageData, int width, int height)
        {
            Throw.IfArgumentNull(imageData, "imageData");

            if (width < 1)
            {
                throw new ArgumentException("The width (" + width + ") must be greater than zero.", nameof(width));
            }

            if (height < 1)
            {
                throw new ArgumentException("The height (" + height + ") must be greater than zero.", nameof(height));
            }

            UIImage image = UIImageByteConverter.FromBytes(imageData);

            if (image.Size.Width != width || image.Size.Height != height)
            {
                RectangleF newBounds =
                    this.CalculateBoundsForScaling(
                        image.CGImage.Width,
                        image.CGImage.Height,
                        image.Orientation,
                        Math.Max(width, height),
                        ScaleMode.Fill);

                return this.Scale(image, new CGSize(width, height), newBounds);
            }

            return imageData;

        }

        /// <summary>
        /// Scales an image to fit within the maximum defined pixel resolution.
        /// </summary>
        /// <param name="imageData">An array containing the image data.</param>
        /// <param name="maxResolution">The maximum resolution, in number of pixels.</param>
        /// <returns>A scalled image.</returns>
        public byte[] ScaleToFit(byte[] imageData, int maxResolution)
        {
            Throw.IfArgumentNull(imageData, "imageData");

            if (maxResolution < 1)
            {
                throw new ArgumentException("The maximum resolution (" + maxResolution + ") must be greater than zero.", nameof(maxResolution));
            }

            UIImage image = UIImageByteConverter.FromBytes(imageData);

            if (image.CGImage.Width > maxResolution || image.CGImage.Height > maxResolution)
            {
                RectangleF newBounds = 
                    this.CalculateBoundsForScaling(
                        image.CGImage.Width, 
                        image.CGImage.Height, 
                        image.Orientation,
                        maxResolution,
                        ScaleMode.Fit);

                return this.Scale(image, newBounds.Size, newBounds);
            }

            return imageData;
        }

        private byte[] Scale(UIImage imageToScale, CGSize destinationSize, CGRect destinationRect)
        {
            UIGraphics.BeginImageContext(destinationSize);

            imageToScale.Draw(destinationRect);

            UIImage scaledImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return UIImageByteConverter.ToBytesAsJpeg(scaledImage, this.compressionQuality);
        }

        private RectangleF CalculateBoundsForScaling(
            float originalWidth, 
            float originalHeight,
            UIImageOrientation originalImageOrientation,
            int resolution,
            ScaleMode scaleMode)
        {
            float width;
            float height;

            if (originalImageOrientation == UIImageOrientation.Up)
            {
                width = originalWidth;
                height = originalHeight;
            }
            else if (originalImageOrientation == UIImageOrientation.Right)
            {
                width = originalHeight;
                height = originalWidth;
            }
            else
            {
                throw new ArgumentException("Unsupported image orientation (" + originalImageOrientation + ").");
            }

            float ratio = width / height;
            SizeF scaledSize = new SizeF();

            if ((ratio > 1 && scaleMode == ScaleMode.Fit) ||
               (ratio < 1 && scaleMode == ScaleMode.Fill))
            {
                scaledSize.Width = resolution;
                scaledSize.Height = resolution / ratio;
            }
            else
            {
                scaledSize.Width = resolution * ratio;
                scaledSize.Height = resolution;
            }

            PointF point = PointF.Empty;
            if (scaleMode == ScaleMode.Fill)
            {
                if (scaledSize.Height > scaledSize.Width)
                {
                    point = new PointF(0, (resolution - scaledSize.Height) * 0.5f);
                }
                else if (scaledSize.Height < scaledSize.Width)
                {
                    point = new PointF((resolution - scaledSize.Width) * 0.5f, 0);
                }
            }

            return new RectangleF(point, scaledSize);
        }
    }
}