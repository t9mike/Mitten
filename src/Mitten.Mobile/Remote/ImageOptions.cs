namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Defines a set of options for retrieving images.
    /// </summary>
    public class ImageOptions
    {
        /// <summary>
        /// Represents an empty set of image options.
        /// </summary>
        public static readonly ImageOptions Empty = new ImageOptions(null, null, ImageResizeMode.Default);

        /// <summary>
        /// Initializes a new instance of the ImageOptions class.
        /// </summary>
        /// <param name="width">An optional width for the image.</param>
        /// <param name="height">An optional height for the image.</param>
        /// <param name="resizeMode">Defines how the image should be resized.</param>
        public ImageOptions(int? width, int? height, ImageResizeMode resizeMode)
        {
            this.Width = width;
            this.Height = height;
            this.ResizeMode = resizeMode;
        }

        /// <summary>
        /// Gets an optional width for the image.
        /// </summary>
        public int? Width { get; private set; }

        /// <summary>
        /// Gets an optional width for the image.
        /// </summary>
        public int? Height { get; private set; }

        /// <summary>
        /// Gets an object that defines how an image will be resized.
        /// </summary>
        public ImageResizeMode ResizeMode { get; private set; }
    }
}