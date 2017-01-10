using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mitten.Mobile.System;

namespace Mitten.Mobile.Remote.Http
{
    public class HttpResourceDownloader : HttpServiceClient, IResourceDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceDownloader"/> class.
        /// </summary>
        /// <param name="messageInvoker">An object used to send server requests.</param>
        /// <param name="networkStatus">An object that provides the current network status.</param>
        public HttpResourceDownloader(HttpMessageInvoker messageInvoker, INetworkStatus networkStatus)
            : base(messageInvoker, networkStatus)
        {
        }

        /// <summary>
        /// Downloads an image located at the specified url.
        /// </summary>
        /// <param name="imageUrl">The url to the image to download.</param>
        /// <param name="options">The options for the image.</param>
        /// <returns>The image.</returns>
        public Task<ServiceResult<byte[]>> DownloadImage(string imageUrl, ImageOptions options)
        {
            Throw.IfArgumentNullOrWhitespace(imageUrl, nameof(imageUrl));
            Throw.IfArgumentNull(options, nameof(options));

            return this.GetResourceAsync(imageUrl, this.GetImageOptionsParameters(options));
        }

        private RequestParameter[] GetImageOptionsParameters(ImageOptions options)
        {
            if (!options.Width.HasValue && !options.Height.HasValue)
            {
                return new RequestParameter[0];
            }

            List<RequestParameter> parameters = new List<RequestParameter>();

            if (options.Width.HasValue)
            {
                parameters.Add(RequestParameter.Create("width", options.Width));
            }

            if (options.Height.HasValue)
            {
                parameters.Add(RequestParameter.Create("height", options.Height));
            }

            if (options.ResizeMode != ImageResizeMode.Default)
            {
                parameters.Add(RequestParameter.Create("mode", options.ResizeMode));
            }

            return parameters.ToArray();
        }
    }
}