using System;

namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Represents a response from a remote server that returned no content.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <param name="failureDetails">The details of a failed service request.</param>
        protected ServiceResult(ServiceResultCode resultCode, string failureDetails)
        {
            this.ResultCode = resultCode;
            this.FailureDetails = failureDetails;
        }

        /// <summary>
        /// Gets the code defining the result of the request.
        /// </summary>
        public ServiceResultCode ResultCode { get; private set; }

        /// <summary>
        /// Gets the details of a failed service request.
        /// </summary>
        public string FailureDetails { get; private set; }

        /// <summary>
        /// Creates a successful response from the server.
        /// </summary>
        public static ServiceResult Success()
        {
            return new ServiceResult(ServiceResultCode.Success, null);
        }

        /// <summary>
        /// Creates a successful response from the server.
        /// </summary>
        /// <param name="response">The response data.</param>
        public static ServiceResult<TResponse> Success<TResponse>(TResponse response)
        {
            return new ServiceResult<TResponse>(response, ServiceResultCode.Success, null);
        }

        /// <summary>
        /// Creates a failed response from the server.
        /// </summary>
        /// <param name="resultCode">A result code defining the reason for the failed response.</param>
        /// <param name="failureDetails">A description of the failure.</param>
        public static ServiceResult Failed(ServiceResultCode resultCode, string failureDetails)
        {
            if (resultCode == ServiceResultCode.Invalid)
            {
                throw new ArgumentException("Result code cannot be Invalid.", nameof(resultCode));
            }

            if (resultCode == ServiceResultCode.Success)
            {
                throw new ArgumentException("Result code cannot be Success, use the ServiceResult.Success factory method instead.", nameof(resultCode));
            }

            return new ServiceResult(resultCode, failureDetails);
        }
    }

    /// <summary>
    /// Represents a response from a remote server.
    /// </summary>
    public class ServiceResult<TResponse> : ServiceResult
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="response">The response from a request.</param>
        /// <param name="resultCode">The result code.</param>
        /// <param name="failureDetails">The failure details.</param>
        internal ServiceResult(TResponse response, ServiceResultCode resultCode, string failureDetails)
            : base(resultCode, failureDetails)
        {
            this.Response = response;
        }

        /// <summary>
        /// Gets the response from a service request.
        /// </summary>
        public TResponse Response { get; private set; }

        /// <summary>
        /// Do not use, instead use the overload that accepts a response value.
        /// </summary>
        public new static ServiceResult<TResponse> Success()
        {
            throw new NotSupportedException("Do not use, instead use the overload that accepts a response value.");
        }

        /// <summary>
        /// Creates a successful response from the server.
        /// </summary>
        /// <param name="response">The response data.</param>
        public static ServiceResult<TResponse> Success(TResponse response)
        {
            return new ServiceResult<TResponse>(response, ServiceResultCode.Success, null);
        }

        /// <summary>
        /// Creates a failed response from the server.
        /// </summary>
        /// <param name="resultCode">A result code defining the reason for the failed response.</param>
        /// <param name="failureDetails">A description of the failure.</param>
        public new static ServiceResult<TResponse> Failed(ServiceResultCode resultCode, string failureDetails)
        {
            if (resultCode == ServiceResultCode.Invalid)
            {
                throw new ArgumentException("Result code cannot be Invalid.", nameof(resultCode));
            }

            if (resultCode == ServiceResultCode.Success)
            {
                throw new ArgumentException("Result code cannot be Success, use the ServiceResult.Success factory method instead.", nameof(resultCode));
            }

            return new ServiceResult<TResponse>(default(TResponse), resultCode, failureDetails);
        }
    }
}