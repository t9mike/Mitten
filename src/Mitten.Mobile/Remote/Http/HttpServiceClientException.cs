using System;

namespace Mitten.Mobile.Remote.Http
{
    /// <summary>
    /// Represents an error that occurred while invoking an http service client.
    /// </summary>
    public class HttpServiceClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the HttpServiceClientException class.
        /// </summary>
        /// <param name="serviceResult">The result from the service.</param>
        /// <param name="message">A message describing the problem.</param>
        public HttpServiceClientException(ServiceResult serviceResult, string message)
            : base(message)
        {
            this.ServiceResult = serviceResult;
        }

        /// <summary>
        /// Gets the result from the service.
        /// </summary>
        public ServiceResult ServiceResult { get; private set; }
    }
}