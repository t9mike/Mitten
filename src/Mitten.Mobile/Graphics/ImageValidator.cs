using Mitten.Mobile.Extensions;

namespace Mitten.Mobile.Graphics
{
    /// <summary>
    /// Handles validating images.
    /// </summary>
    public static class ImageValidator
    {
        private static class Constants
        {
            public static readonly byte[] JpegStartOfImage = { 0xFF, 0xD8 };
            public static readonly byte[] JpegEndOfImage = { 0xFF, 0xD9 };
        }

        /// <summary>
        /// Validates that the specified array of bytes contains a jpeg image.
        /// </summary>
        /// <param name="image">An array of bytes.</param>
        /// <returns>True if the array of bytes contains a valid jpeg.</returns>
        public static bool IsValidJpeg(byte[] image)
        {
            return 
                image != null &&
                image.StartsWith(Constants.JpegStartOfImage) &&
                image.EndsWith(Constants.JpegEndOfImage);
        }
    }
}