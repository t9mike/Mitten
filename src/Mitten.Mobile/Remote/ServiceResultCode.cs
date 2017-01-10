namespace Mitten.Mobile.Remote
{
    /// <summary>
    /// Defines result codes representing general remote service outcomes.
    /// </summary>
    public enum ServiceResultCode
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The request was a success.
        /// </summary>
        Success,

        /// <summary>
        /// The user making the request is unauthorized.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// The user made a bad request to the server, e.g. validation issue or missing data.
        /// </summary>
        BadRequest,

        /// <summary>
        /// Indicates that the request has timed out.
        /// </summary>
        RequestTimeout,

        /// <summary>
        /// The content from a server response was invalid or unexpected, e.g. the response content failed to deserialize.
        /// </summary>
        InvalidResponseContent,

        /// <summary>
        /// There was a conflict with the current state of the server, e.g. duplicate email when registering a new user.
        /// </summary>
        Conflict,

        /// <summary>
        /// Inidicates that a requested resource on the server was not found.
        /// </summary>
        ResourceNotFound,

        /// <summary>
        /// An internal error occurred on the server.
        /// </summary>
        InternalServerError,

        /// <summary>
        /// Indicates that the request could not be made because the network is currently unavailable.
        /// </summary>
        NetworkUnavailable,

        /// <summary>
        /// Indicates that the request requires a wifi connection.
        /// </summary>
        WifiRequired,

        /// <summary>
        /// A failure that occurred while trying to establish a connection with the remote server.
        /// </summary>
        ConnectionFailure,
    
        /// <summary>
        /// A failure in the network while communicating with the remote server.
        /// </summary>
        CommunicationFailure,

        /// <summary>
        /// An unknown status was returned from the server.
        /// </summary>
        Unknown,
    }
}