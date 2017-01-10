using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Manages concurrent downloading and caching of remote images. 
    /// 
    /// Retry policy: 
    /// 
    /// All requests get cached regardless of the result. Subsequent requests for the image will either
    /// retrieve the request from cache (if the original request was successful) or attempt a
    /// retry as long as the number of retries for the request does not exceed a defined
    /// threshold. If the number of retries is at its max, the cached failed request will be
    /// returned.
    /// 
    /// There are 2 exceptions to the retry policy: 
    /// 1) If the request failed because the resource was not found, the request will not be retried.
    /// 2) If the request failed due to the network being unavailable, the retry counter will not be incremented.
    /// </summary>
    public class RemoteImageCatalog
    {
        private static class Constants
        {
            public const int MaximumConcurrentDownloads = 3;
            public const int RetryAttempts = 3;
        }

        private readonly ConcurrentDictionary<string, RemoteImageRequest> imageRequests;

        private readonly IResourceDownloader resourceDownloader;
        private readonly ICache<byte[]> imageCache;
        private readonly object startDownloadSync;
        private readonly int maxConcurrentDownloads;
        private readonly int maxRetryAttempts;

        private volatile bool isStartingDownloadsInProgress;
        private int currentDownloadCount;

        /// <summary>
        /// Initializes a new instance of the RemoteImageCatalog class.
        /// </summary>
        /// <param name="resourceDownloader">A resource downloader used to download resources.</param>
        /// <param name="imageCache">Handles caching downloaded images.</param>
        /// <param name="maxConcurrentDownloads">The maximum number of allowed concurrent downloads.</param>
        /// <param name="maxRetryAttempts">The number of retries for a failed request.</param>
        public RemoteImageCatalog(
            IResourceDownloader resourceDownloader, 
            ICache<byte[]> imageCache,
            int maxConcurrentDownloads = Constants.MaximumConcurrentDownloads,
            int maxRetryAttempts = Constants.RetryAttempts)
        {
            Throw.IfArgumentNull(resourceDownloader, nameof(resourceDownloader));
            Throw.IfArgumentNull(imageCache, nameof(imageCache));

            if (maxConcurrentDownloads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxConcurrentDownloads), "The maximum number of concurrent downloads must be greater than 0.");
            }

            if (maxRetryAttempts < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetryAttempts), "The maximum number of retries must be greater than or equal to 0.");
            }

            this.resourceDownloader = resourceDownloader;
            this.imageCache = imageCache;
            this.maxConcurrentDownloads = maxConcurrentDownloads;
            this.maxRetryAttempts = maxRetryAttempts;

            this.startDownloadSync = new object();
            this.imageRequests = new ConcurrentDictionary<string, RemoteImageRequest>();
        }

        /// <summary>
        /// Gets the number of downloads currently in progress.
        /// </summary>
        public int DownloadsInProgressCount
        {
            get { return this.currentDownloadCount; }
        }

        private bool IsAtMaxDownload
        {
            get { return this.currentDownloadCount == this.maxConcurrentDownloads; }
        }

        /// <summary>
        /// Attempts to get an image from the catalog for the specified url.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The image options.</param>
        /// <returns>The image if one is currently stored in the catalog or null if one does not exist.</returns>
        public byte[] TryGetImage(string imageUrl, ImageOptions options)
        {
            return this.imageCache.TryGet(this.GetRequestKey(imageUrl, options));
        }

        /// <summary>
        /// Attempts to get an image from the catalog for the specified url.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The image options.</param>
        /// <param name="convert">A method to convert the bytes into a platform specific image object.</param>
        /// <returns>The image if one is currently stored in the catalog or null if one does not exist.</returns>
        public TImage TryGetImage<TImage>(string imageUrl, ImageOptions options, Func<byte[], TImage> convert)
            where TImage : class
        {
            byte[] data = this.imageCache.TryGet(this.GetRequestKey(imageUrl, options));

            return
                data != null
                ? convert(data)
                : null;
        }

        /// <summary>
        /// Adds the specified image into the cache if it does not exist or updates it if one already exists.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The image options for the image.</param>
        /// <param name="imageData">The image as bytes.</param>
        public void AddOrUpdateCachedImage(string imageUrl, ImageOptions options, byte[] imageData)
        {
            string key = this.GetRequestKey(imageUrl, options);
            this.imageCache.Put(key, imageData);
        }

        /// <summary>
        /// Removes the specified image from the cache if it exists.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The image options for the image.</param>
        public void RemoveCachedImage(string imageUrl, ImageOptions options)
        {
            string key = this.GetRequestKey(imageUrl, options);
            this.imageCache.Remove(key);
        }

        /// <summary>
        /// Deletes all images saved in the cache.
        /// </summary>
        public void ClearCache()
        {
            this.imageCache.Clear();
        }

        /// <summary>
        /// Gets whether or not an image for the specified url is currently stored in cache.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The options for the image.</param>
        /// <returns>True if the image for the url is currently stored in cache, otherwise false.</returns>
        public bool IsInCache(string imageUrl, ImageOptions options)
        {
            string requestKey = this.GetRequestKey(imageUrl, options);
            return this.imageCache.Exists(requestKey);
        }

        /// <summary>
        /// Gets a task that will wait until all the current image downloads complete.
        /// </summary>
        /// <returns>A task.</returns>
        public Task GetImageDownloadsInProgressTask()
        {
            IEnumerable<Task> inProgressTasks = 
                this.imageRequests
                .Where(item => !item.Value.IsRequestCompleted)
                .Select(item => item.Value.Task);

            return Task.WhenAll(inProgressTasks);
        }

        /// <summary>
        /// Starts a new request for an image at the specified image url. If an image is already
        /// cached, a new request will still be made and replace the current image saved in cache.
        /// </summary>
        /// <param name="imageUrl">The url for the image.</param>
        /// <param name="options">The image options.</param>
        /// <returns>The request.</returns>
        public RemoteImageRequest StartImageRequest(string imageUrl, ImageOptions options)
        {
            Throw.IfArgumentNullOrWhitespace(imageUrl, "imageUrl");
            Throw.IfArgumentNull(options, "options");

            string requestKey = this.GetRequestKey(imageUrl, options);

            RemoteImageRequest request;
            if (this.imageRequests.TryGetValue(requestKey, out request))
            {
                if (request.IsRequestCompleted &&
                    request.Task.Result.ResultCode != ServiceResultCode.Success)
                {
                    return this.AttemptRetry(request, imageUrl, options);
                }

                return request;
            }

            return this.TryStartImageRequest(imageUrl, options);
        }

        /// <summary>
        /// Gets the key for the specified url and options.
        /// </summary>
        /// <param name="imageUrl">An image url.</param>
        /// <param name="options">Image options.</param>
        /// <returns>The request key.</returns>
        protected string GetRequestKey(string imageUrl, ImageOptions options)
        {
            StringBuilder key = new StringBuilder(imageUrl);

            if (options.Width.HasValue)
            {
                key.Append("w=" + options.Width.Value);
            }

            if (options.Height.HasValue)
            {
                key.Append("h=" + options.Height.Value);
            }

            if (options.ResizeMode != ImageResizeMode.Default)
            {
                key.Append("mode=" + options.ResizeMode);
            }

            return key.ToString();
        }

        private RemoteImageRequest AttemptRetry(RemoteImageRequest failedRequest, string imageUrl, ImageOptions options)
        {
            if (!failedRequest.IsRequestCompleted)
            {
                throw new ArgumentException("The request has not completed.", nameof(failedRequest));
            }

            if (failedRequest.Task.Result.ResultCode == ServiceResultCode.Success)
            {
                throw new ArgumentException("The request was successful, no retry needed.", nameof(failedRequest));
            }

            if (failedRequest.Task.Result.ResultCode == ServiceResultCode.ResourceNotFound ||
                failedRequest.RetryCounter == this.maxRetryAttempts)
            {
                return failedRequest;
            }

            int retryCounter = failedRequest.RetryCounter;
            if (failedRequest.Task.Result.ResultCode != ServiceResultCode.NetworkUnavailable)
            {
                // If the network is unavailable we will keep retrying until it becomes available.
                // It is assumed that the underlying remote client is optimized to handle network status
                // so that a call to make repeated remote request is cheap.

                retryCounter++;
            }

            string requestKey = this.GetRequestKey(imageUrl, options);
            this.TryRemoveRequest(requestKey);

            return this.TryStartImageRequest(imageUrl, options, retryCounter);
        }

        private RemoteImageRequest TryStartImageRequest(string imageUrl, ImageOptions options, int retryCounter = 0)
        {
            string requestKey = this.GetRequestKey(imageUrl, options);

            RemoteImageRequest requestToStart =
                this.imageRequests.GetOrAdd(
                    requestKey,
                    _ =>
                        new RemoteImageRequest(
                            () => this.resourceDownloader.DownloadImage(imageUrl, options),
                            result => this.DownloadComplete(requestKey, result),
                            imageData => this.imageCache.Put(requestKey, imageData),
                            retryCounter));

            this.StartPendingDownloadsAsync();

            return requestToStart;
        }

        private void DownloadComplete(string requestKey, ServiceResult result)
        {
            if (result.ResultCode == ServiceResultCode.Success)
            {
                this.TryRemoveRequest(requestKey);
            }

            Interlocked.Decrement(ref this.currentDownloadCount);
            this.StartPendingDownloadsAsync();
        }

        private void TryRemoveRequest(string key)
        {
            RemoteImageRequest notUsed;
            this.imageRequests.TryRemove(key, out notUsed);
        }

        private void StartPendingDownloadsAsync()
        {
            if (!this.isStartingDownloadsInProgress && !this.IsAtMaxDownload)
            {
                lock (this.startDownloadSync)
                {
                    this.isStartingDownloadsInProgress = true;

                    foreach (RemoteImageRequest pendingRequest in 
                        this.imageRequests.Where(item => item.Value.IsPendingDownload).Select(item => item.Value))
                    {
                        if (this.IsAtMaxDownload)
                        {
                            break;
                        }

                        if (pendingRequest.TryStartImageDownload())
                        {
                            Interlocked.Increment(ref this.currentDownloadCount);
                        }
                    }

                    this.isStartingDownloadsInProgress = false;
                }
            }
        }
    }
}