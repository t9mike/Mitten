namespace Mitten.Mobile.Graphics
{
    /// <summary>
    /// Handles image transoformations.
    /// </summary>
    public interface IImageTransform
    {
        /// <summary>
        /// Scales an image to fill the specified bounds while maintaining aspect ratio.
        /// Any part of the image that spills outside the bounds will be cropped out.
        /// </summary>
        /// <param name="imageData">An array containing the image data.</param>
        /// <param name="width">The width, as the number of pixels, to for the bounds.</param>
        /// <param name="height">The height, as the number of pixels, to for the bounds.</param>
        /// <returns>A scaled image.</returns>
        byte[] ScaleToFill(byte[] imageData, int width, int height);

        /// <summary>
        /// Scales an image to fit within the maximum defined pixel resolution.
        /// </summary>
        /// <param name="imageData">An array containing the image data.</param>
        /// <param name="maxResolution">The maximum resolution, in number of pixels.</param>
        /// <returns>A scalled image.</returns>
        byte[] ScaleToFit(byte[] imageData, int maxResolution);
    }
}