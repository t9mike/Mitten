using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Represents an async request for a remote image.
    /// </summary>
    public class RemoteImageRequest
    {
        private static class Constants
        {
            public const int FalseInt = 0;
            public const int TrueInt = 1;
        }

        private readonly Func<Task<ServiceResult<byte[]>>> startDownload;
        private readonly TaskCompletionSource<ServiceResult> taskCompletionSource;
        private readonly Action<ServiceResult> downloadComplete;
        private readonly Action<byte[]> processImage;

        private int hasDownloadStarted;

        /// <summary>
        /// Initializes a new instance of the RemoteImageRequest class.
        /// </summary>
        /// <param name="startDownload">Start download.</param>
        /// <param name="downloadComplete">Download complete.</param>
        /// <param name="processImage">Processes the image if it was downloaded successfully.</param>
        /// <param name="retryCounter">A counter used to identify the number of retries for a request.</param>
        internal RemoteImageRequest(
            Func<Task<ServiceResult<byte[]>>> startDownload, 
            Action<ServiceResult> downloadComplete, 
            Action<byte[]> processImage,
            int retryCounter = 0)
        {
            this.startDownload = startDownload;
            this.downloadComplete = downloadComplete;
            this.processImage = processImage;
            this.taskCompletionSource = new TaskCompletionSource<ServiceResult>();
            this.RetryCounter = retryCounter;
        }

        /// <summary>
        /// Gets whether or not the request is currently queued for download.
        /// </summary>
        public bool IsPendingDownload 
        { 
            get { return this.hasDownloadStarted == Constants.FalseInt; }
        }

        /// <summary>
        /// Gets whether or not the request has completed.
        /// </summary>
        public bool IsRequestCompleted
        {
            get { return this.Task.IsCompleted; }
        }

        /// <summary>
        /// Gets the task for the request, this encapsulates the wait time in the queue and actual remote call.
        /// </summary>
        public Task<ServiceResult> Task 
        { 
            get { return this.taskCompletionSource.Task; }
        }

        /// <summary>
        /// Gets the retry attempt counter for the request.
        /// </summary>
        internal int RetryCounter { get; private set; }

        /// <summary>
        /// Starts the image download.
        /// </summary>
        /// <returns>True if the image download was started; false if a download is already in progress or has completed.</returns>
        internal bool TryStartImageDownload()
        {
            if (!this.AquireForDownload())
            {
                return false;
            }

            this.startDownload().ContinueWith(task => this.HandleDownloadTaskFinished(task.Result));
            return true;
        }

        private void HandleDownloadTaskFinished(ServiceResult<byte[]> result)
        {
            if (result.ResultCode == ServiceResultCode.Success)
            {
                try
                {
                    this.processImage(result.Response);
                    this.taskCompletionSource.SetResult(ServiceResult.Success());
                }
                catch (Exception ex)
                {
                    this.taskCompletionSource.SetResult(ServiceResult.Failed(ServiceResultCode.InvalidResponseContent, "Failed to processes the received bytes with exception: " + ex));
                }
            }
            else
            {
                this.taskCompletionSource.SetResult(ServiceResult.Failed(result.ResultCode, result.FailureDetails));
            }

            this.downloadComplete(result);
        }

        private bool AquireForDownload()
        {
            return Interlocked.CompareExchange(ref this.hasDownloadStarted, Constants.TrueInt, Constants.FalseInt) == Constants.FalseInt;
        }
    }
}