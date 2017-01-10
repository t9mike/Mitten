using System;
using System.Runtime.InteropServices;
using Foundation;
using Mitten.Mobile.iOS.Views;
using UIKit;

namespace Mitten.Mobile.iOS.Graphics
{
    /// <summary>
    /// Utility class for converting UIImages to and from a byte array.
    /// </summary>
    public static class UIImageByteConverter
    {
        private static class Constants
        {
            public const float DefaultCompressionQuality = 0.5f;
        }

        /// <summary>
        /// Converts a byte array to a UIImage object.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="decompress">
        /// True if the image should be decompressed, otherwise false. This would be useful when converting
        /// a byte array into a UIImage for a UIImageView on a background thread. The image would otherwise get 
        /// decompressed by the UIImageView synchronously on the UI thread. Decompressing it on a background
        /// thread will create a smoother UX.
        /// </param>
        /// <returns>A new UIImage instance.</returns>
        public static UIImage FromBytes(byte[] bytes, bool decompress = false)
        {
            UIImage image = UIImage.LoadFromData(NSData.FromArray(bytes));

            return
                decompress
                ? image.Decompress()
                : image;
        }

        /// <summary>
        /// Converts the image to an array of bytes containing a jpeg image.
        /// </summary>
        /// <param name="image">The image to convert.</param>
        /// <param name="compressionQuality">The quality of the of the resulting image, ranging from 0.0 - 1.0 where 0.0 represents the maximum compression level; the default is 0.5.</param>
        /// <returns>An array of bytes containing the data for a jpeg image.</returns>
        public static byte[] ToBytesAsJpeg(UIImage image, float compressionQuality = Constants.DefaultCompressionQuality)
        {
            using (NSData imageData = image.AsJPEG(compressionQuality)) 
            {
                byte[] bytes = new byte[imageData.Length];

                Marshal.Copy(
                    imageData.Bytes, 
                    bytes, 
                    0, 
                    Convert.ToInt32(imageData.Length));
            
                return bytes;
            }
        }
    }
}