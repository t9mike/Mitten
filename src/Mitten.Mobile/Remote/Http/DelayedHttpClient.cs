using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mitten.Mobile.Remote.Http
{
    /// <summary>
    /// An http client that adds a delay before sending messages.
    /// </summary>
    public class DelayedHttpClient : HttpClient
    {
        private readonly TimeSpan delay;

        /// <summary>
        /// Initializes a new instance of the DelayedHttpClient class.
        /// </summary>
        /// <param name="delay">The amount of time the invoker should delay when sending a request.</param>
        public DelayedHttpClient(TimeSpan delay)
        {
            this.delay = delay;
        }

        /// <summary>
        /// Initializes a new instance of the DelayedHttpClient class.
        /// </summary>
        /// <param name="handler">The handler responsible for processing the HTTP response messages.</param>
        /// <param name="delay">The amount of time the invoker should delay when sending a request.</param>
        public DelayedHttpClient(HttpMessageHandler handler, TimeSpan delay)
            : base(handler)
        {
            this.delay = delay; 
        }

        /// <summary>
        /// Sends an HTTP request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(this.delay).ConfigureAwait(false);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}