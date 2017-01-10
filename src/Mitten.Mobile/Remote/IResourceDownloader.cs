using System.Threading.Tasks;

namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Defines an object that handles downloading resources.
    /// </summary>
    public interface IResourceDownloader
    {
        /// <summary>
        /// Downloads an image located at the specified url.
        /// </summary>
        /// <param name="imageUrl">The url to the image to download.</param>
        /// <param name="options">The options for the image.</param>
        /// <returns>The image.</returns>
        Task<ServiceResult<byte[]>> DownloadImage(string imageUrl, ImageOptions options);
    }
}