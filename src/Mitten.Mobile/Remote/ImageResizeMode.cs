namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Defines how an image will be handled when dealing with aspect ratio conflicts during resizing.
    /// </summary>
    public enum ImageResizeMode
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Indicates that the default resizing mode on the server should be used.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates that the image will be cropped to fit within the bounds.
        /// </summary>
        Crop,

        /// <summary>
        /// Indicates that the resulting image will have the same dimensions as the specified bounds but the aspect ratio will be maintained and any extra space will be padded with a solid color.
        /// </summary>
        Pad,

        /// <summary>
        /// Indicates that an image will be scaled to fit within the bounds of the specified dimensions.
        /// </summary>
        Scale,
    }
}