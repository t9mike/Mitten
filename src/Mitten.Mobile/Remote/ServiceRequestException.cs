using System;

namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Represents an error due to a failed service request.
    /// </summary>
    public class ServiceRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ServiceRequestException class.
        /// </summary>
        /// <param name="serviceResult">A service result for the failed request.</param>
        public ServiceRequestException(ServiceResult serviceResult)
            : base ("Failed while invoking a service request with code (" + serviceResult.ResultCode + ") and error: " + serviceResult.FailureDetails)
        {
            this.ServiceResult = serviceResult;
        }

        /// <summary>
        /// Gets the service result for the failed request.
        /// </summary>
        public ServiceResult ServiceResult { get; private set; }
    }
}