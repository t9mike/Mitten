using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mitten.Mobile.Remote.Http.Json;
using Mitten.Mobile.System;
using Newtonsoft.Json;

namespace Mitten.Mobile.Remote.Http
{
    /// <summary>
    /// Base class for all clients communicating with our servers using RESTful service calls.
    /// </summary>
    public abstract class HttpServiceClient  : IDisposable
    {
        private static class Constants
        {
            public const string QueryPrefix = "?";
            public const string ValueSeperator = "=";
            public const string ArgumentSeperator = "&";

            public const string TextContentType = "text";
            public const string ImageJpgContentType = "image/jpeg";
            public const string ApplicationJsonContentType = "application/json";
            public const string FormContentDispositionType = "form-data";
            public const string TicketImageContentKey = "image";

            public const string UserAgent = "Finch-mobile";
            public const string AuthenticationHeaderPrefix = "Bearer";

            public const string HttpPatchMethod = "PATCH";

            public const string Boundary = "----FinchBoundary";
            public const int BoundaryPadLength = 16;
            public static readonly char[] BoundaryCharacters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        }

        // the HttpClient will throw web exceptions with status codes that msdn says are not supported in PCL
        private static class ExtendedWebExceptionStatus
        {
            public const int NameResolutionFailure = 1;
            public const int ReceiveFailure = 3;
        }

        private readonly HttpMessageInvoker messageInvoker;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceClient"/> class.
        /// </summary>
        /// <param name="messageInvoker">An object used to send server requests.</param>
        /// <param name="networkStatus">An object that provides the current network status.</param>
        protected HttpServiceClient(HttpMessageInvoker messageInvoker, INetworkStatus networkStatus)
        {
            Throw.IfArgumentNull(messageInvoker, "client");
            Throw.IfArgumentNull(networkStatus, "networkStatus");

            this.messageInvoker = messageInvoker;
            this.NetworkStatus = networkStatus;
        }

        /// <summary>
        /// Gets an objec that provides the current network status.
        /// </summary>
        protected INetworkStatus NetworkStatus { get; }

        void IDisposable.Dispose()
        {
            if (!this.isDisposed)
            {
                this.messageInvoker.Dispose();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Makes a GET request to the service for a resource returned as an array of bytes.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<byte[]>> GetResourceAsync(string uri, params RequestParameter[] parameters)
        {
            return this.GetResourceAsync(uri, null, parameters);
        }

        /// <summary>
        /// Makes a GET request to the service for a resource returned as an array of bytes.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<byte[]>> GetResourceAsync(string uri, string securityToken, params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            string fullUri = uri + HttpServiceClient.BuildQuery(parameters);
            HttpRequestMessage request = HttpServiceClient.CreateGetRequest(fullUri, securityToken);
            return this.MakeServiceRequestWithBinaryResponse(request);  
        }

        /// <summary>
        /// Makes a GET request to the service with an expected Json response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<TResponse>> GetFromJsonAsync<TResponse>(string uri, string securityToken, params RequestParameter[] parameters)
        {
            return 
                this.GetFromJsonAsync<TResponse, TResponse>(
                    uri, 
                    securityToken, 
                    r => r,
                    parameters);   
        }

        /// <summary>
        /// Makes a GET request to the service with an expected Json response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="serializerSettings">Serializer settings used when deserializing the response content from the server.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<TResponse>> GetFromJsonAsync<TResponse>(
            string uri, 
            string securityToken,
            JsonSerializerSettings serializerSettings,
            params RequestParameter[] parameters)
        {
            return 
                this.GetFromJsonAsync<TResponse, TResponse>(
                    uri, 
                    securityToken,
                    serializerSettings,
                    r => r,
                    parameters);   
        }

        /// <summary>
        /// Makes a GET request to the service with an expected Json response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="convertFromDTO">Converts a received DTO into the expected response type.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<TResponse>> GetFromJsonAsync<TResponse, TDTO>(
            string uri, 
            string securityToken, 
            Func<TDTO, TResponse> convertFromDTO,
            params RequestParameter[] parameters)
        {
            return 
                this.GetFromJsonAsync<TResponse, TDTO>(
                    uri, 
                    securityToken,
                    HttpServiceClient.GetSerializerSettings(),
                    convertFromDTO,
                    parameters); 
        }

        /// <summary>
        /// Makes a GET request to the service with an expected Json response.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="serializerSettings">Serializer settings used when deserializing the response content from the server.</param>
        /// <param name="convertFromDTO">Converts a received DTO into the expected response type.</param>
        /// <param name="parameters">Optional parameters to pass as part of the query string for the URI.</param>
        /// <returns>The result of the GET request.</returns>
        protected Task<ServiceResult<TResponse>> GetFromJsonAsync<TResponse, TDTO>(
            string uri, 
            string securityToken, 
            JsonSerializerSettings serializerSettings,
            Func<TDTO, TResponse> convertFromDTO,
            params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            string fullUri = uri + HttpServiceClient.BuildQuery(parameters);
            HttpRequestMessage request = HttpServiceClient.CreateGetRequest(fullUri, securityToken);
            return this.MakeServiceRequestWithJsonResponse<TResponse, TDTO>(request, serializerSettings, convertFromDTO);
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult<TResponse>> PostAsync<TResponse>(string uri, string securityToken, params RequestParameter[] parameters)
        {
            return 
                this.PostAsync<TResponse, TResponse>(
                    uri, 
                    securityToken, 
                    r => r,
                    parameters); 
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="convertFromDTO">Converts a received DTO into the expected response type.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult<TResponse>> PostAsync<TResponse, TDTO>(
            string uri, 
            string securityToken, 
            Func<TDTO, TResponse> convertFromDTO,
            params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = HttpServiceClient.CreateFormPostRequest(uri, securityToken, parameters);
            return this.MakeServiceRequestWithJsonResponse<TResponse, TDTO>(request, HttpServiceClient.GetSerializerSettings(), convertFromDTO);
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult<TResponse>> PostAsync<TResponse>(
            string uri, 
            byte[] file,
            string fileName,
            string securityToken, 
            params RequestParameter[] parameters)
        {
            return
                this.PostAsync<TResponse, TResponse>(
                    uri,
                    file,
                    fileName,
                    securityToken,
                    r => r,
                    parameters);
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="convertFromDTO">Converts a received DTO into the expected response type.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult<TResponse>> PostAsync<TResponse, TDTO>(
            string uri,
            byte[] file,
            string fileName,
            string securityToken,
            Func<TDTO, TResponse> convertFromDTO,
            params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = HttpServiceClient.CreateMultipartPostRequest(uri, file, fileName, securityToken, parameters);
            return this.MakeServiceRequestWithJsonResponse<TResponse, TDTO>(request, HttpServiceClient.GetSerializerSettings(), convertFromDTO);
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult> PostAsync(string uri, string securityToken, params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = HttpServiceClient.CreateFormPostRequest(uri, securityToken, parameters);
            return this.MakeServiceRequest(request);
        }

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="file">An array of bytes containing the data of a file.</param>
        /// <param name="fileName">The name of the file including an extension identifying the type of file.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="parameters">The parameters to pass as part of a form.</param>
        /// <returns>The result of the POST request.</returns>
        protected Task<ServiceResult> PostAsync(
            string uri, 
            byte[] file, 
            string fileName,
            string securityToken,
            params RequestParameter[] parameters)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = HttpServiceClient.CreateMultipartPostRequest(uri, file, fileName, securityToken, parameters);
            return this.MakeServiceRequest(request);
        }

        /// <summary>
        /// Makes a POST request to the service passing a file.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="file">The file data to post.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The result of the POST request along with the location for the file.</returns>
        protected Task<ServiceResult<string>> PostFileAsync(string uri, string securityToken, byte[] file, string fileName)
        {
            HttpRequestMessage request = HttpServiceClient.CreateMultipartPostRequest(uri, file, fileName, securityToken);

            return
                this.MakeServiceRequest<ServiceResult<string>>(
                    request,
                    goodResponse => Task.FromResult(ServiceResult<string>.Success(HttpServiceClient.TryGetLocationFromResponseHeader(goodResponse))),
                    badResponse => ServiceResult<string>.Failed(HttpServiceClient.GetServiceResultCode(badResponse), HttpServiceClient.GetFailureDetails(badResponse)),
                    () => ServiceResult<string>.Failed(ServiceResultCode.NetworkUnavailable, null),
                    ServiceResult<string>.Failed);
        }

        /// <summary>
        /// Makes a POST request to the service passing the specified content as a Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="content">The content to send as the body of the request.</param>
        protected Task<ServiceResult> PostAsJsonAsync(string uri, string securityToken, dynamic content)
        {
            return this.PostAsJsonAsync<dynamic>(uri, securityToken, content);
        }

        /// <summary>
        /// Makes a POST request to the service passing the specified content as a Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="content">The content to send as the body of the request.</param>
        protected Task<ServiceResult> PostAsJsonAsync<TRequest>(string uri, string securityToken, TRequest content)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = this.CreateJsonRequest(HttpMethod.Post, uri, securityToken, content);
            return this.MakeServiceRequest(request);
        }

        /// <summary>
        /// Makes a POST request to the service passing the specified content as a Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="content">The content to send as the body of the request.</param>
        protected Task<ServiceResult<TResponse>> PostAsJsonAsync<TRequest, TResponse>(string uri, string securityToken, TRequest content)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = this.CreateJsonRequest(HttpMethod.Post, uri, securityToken, content);
            return this.MakeServiceRequestWithJsonResponse<TResponse>(request);
        }

        /// <summary>
        /// Makes a POST request to the service passing the specified Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="json">A json string to send as the body of the request.</param>
        protected Task<ServiceResult<TResponse>> PostJsonAsync<TResponse>(string uri, string securityToken, string json)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = this.CreateJsonRequest(HttpMethod.Post, uri, securityToken, json);
            return this.MakeServiceRequestWithJsonResponse<TResponse>(request);
        }

        /// <summary>
        /// Makes a PATCH request to the service passing the specified content as a Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="content">The content to send as the body of the request.</param>
        protected Task<ServiceResult<TResponse>> PatchAsJsonAsync<TRequest, TResponse>(string uri, string securityToken, TRequest content)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = this.CreateJsonRequest(new HttpMethod(Constants.HttpPatchMethod), uri, securityToken, content);
            return this.MakeServiceRequestWithJsonResponse<TResponse>(request);
        }

        /// <summary>
        /// Makes a PUT request to the service passing the specified content as a Json string in the request body.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <param name="content">The content to send as the body of the request.</param>
        protected Task<ServiceResult> PutAsJsonAsync<TRequest>(string uri, string securityToken, TRequest content)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = this.CreateJsonRequest(HttpMethod.Put, uri, securityToken, content);
            return this.MakeServiceRequest(request);
        }

        /// <summary>
        /// Makes a DELETE request.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        /// <param name="securityToken">A security token used by the server to validate the client, or null if making the call anonymously.</param>
        /// <returns>The result of the DELETE request.</returns>
        protected Task<ServiceResult> DeleteAsync(string uri, string securityToken)
        {
            this.ThrowIfDisposed();

            HttpRequestMessage request = HttpServiceClient.GetRequestTemplate(HttpMethod.Delete, uri, securityToken);
            return this.MakeServiceRequest(request);
        }

        /// <summary>
        /// Serializes the specified object into a json string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A json string.</returns>
        protected string SerializeToJson(object value)
        {
            return JsonConvert.SerializeObject(value, HttpServiceClient.GetSerializerSettings());
        }

        /// <summary>
        /// Throws if this object instance has already been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private Task<ServiceResult> MakeServiceRequest(HttpRequestMessage request)
        {
            return
                this.MakeServiceRequest<ServiceResult>(
                    request,
                    goodResponse => Task.FromResult(ServiceResult.Success()),
                    badResponse => ServiceResult.Failed(HttpServiceClient.GetServiceResultCode(badResponse), HttpServiceClient.GetFailureDetails(badResponse)),
                    () => ServiceResult.Failed(ServiceResultCode.NetworkUnavailable, null),
                    ServiceResult.Failed);
        }

        private Task<ServiceResult<byte[]>> MakeServiceRequestWithBinaryResponse(HttpRequestMessage request)
        {
            return 
                this.MakeServiceRequest<ServiceResult<byte[]>>(
                    request,
                    async goodResponse => ServiceResult<byte[]>.Success(await HttpServiceClient.GetResponseAsBytes(goodResponse).ConfigureAwait(false)),
                    badResponse => ServiceResult<byte[]>.Failed(HttpServiceClient.GetServiceResultCode(badResponse), HttpServiceClient.GetFailureDetails(badResponse)),
                    () => ServiceResult<byte[]>.Failed(ServiceResultCode.NetworkUnavailable, null),
                    ServiceResult<byte[]>.Failed);
        }

        private Task<ServiceResult<TResponse>> MakeServiceRequestWithJsonResponse<TResponse>(HttpRequestMessage request)
        {
            return this.MakeServiceRequestWithJsonResponse<TResponse>(request, HttpServiceClient.GetSerializerSettings());
        }

        private Task<ServiceResult<TResponse>> MakeServiceRequestWithJsonResponse<TResponse>(HttpRequestMessage request, JsonSerializerSettings serializerSettings)
        {
            return this.MakeServiceRequestWithJsonResponse<TResponse, TResponse>(request, serializerSettings, r => r);
        }

        private Task<ServiceResult<TResponse>> MakeServiceRequestWithJsonResponse<TResponse, TDTO>(HttpRequestMessage request, JsonSerializerSettings serializerSettings, Func<TDTO, TResponse> convertFromDTO)
        {
            return 
                this.MakeServiceRequest<ServiceResult<TResponse>>(
                    request,
                    async goodResponse => ServiceResult<TResponse>.Success(await HttpServiceClient.GetResponseFromJson<TResponse, TDTO>(goodResponse, serializerSettings, convertFromDTO).ConfigureAwait(false)),
                    badResponse => ServiceResult<TResponse>.Failed(HttpServiceClient.GetServiceResultCode(badResponse), HttpServiceClient.GetFailureDetails(badResponse)),
                    () => ServiceResult<TResponse>.Failed(ServiceResultCode.NetworkUnavailable, null),
                    ServiceResult<TResponse>.Failed);
        }

        private async Task<TServiceResult> MakeServiceRequest<TServiceResult>(
            HttpRequestMessage request, 
            Func<HttpResponseMessage, Task<TServiceResult>> handleSuccessfulResponse,
            Func<HttpResponseMessage, TServiceResult> handleFailedResponse,
            Func<TServiceResult> handleNetworkUnavailable,
            Func<ServiceResultCode, string, TServiceResult> handleException)
        {
            try
            {
                using (request)
                {
                    if (this.NetworkStatus.GetNetworkAvailability() == NetworkAvailability.NotAvailable)
                    {
                        return handleNetworkUnavailable();
                    }

                    using (HttpResponseMessage response = await this.messageInvoker.SendAsync(request, CancellationToken.None).ConfigureAwait(false))
                    {
                        return
                            response.IsSuccessStatusCode
                            ? await handleSuccessfulResponse(response).ConfigureAwait(false)
                            : handleFailedResponse(response);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                return handleException(ServiceResultCode.InvalidResponseContent, ex.Message);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure ||
                    (int)ex.Status == ExtendedWebExceptionStatus.NameResolutionFailure)
                {
                    return handleException(ServiceResultCode.ConnectionFailure, ex.Message);
                }

                if (ex.Status == WebExceptionStatus.RequestCanceled ||
                    ex.Status == WebExceptionStatus.SendFailure ||
                    (int)ex.Status == ExtendedWebExceptionStatus.ReceiveFailure)
                {
                    return handleException(ServiceResultCode.CommunicationFailure, ex.Message);
                }

                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (!ex.CancellationToken.IsCancellationRequested)
                {
                    // We will get here if the HttpClient/MessageInvoker times out waiting for a server response when calling SendAsync.
                    return handleException(ServiceResultCode.RequestTimeout, ex.Message);
                }

                throw;
            }
        }

        private HttpRequestMessage CreateJsonRequest(HttpMethod method, string uri, string securityToken, object content)
        {
            return this.CreateJsonRequest(method, uri, securityToken, this.SerializeToJson(content));
        }

        private HttpRequestMessage CreateJsonRequest(HttpMethod method, string uri, string securityToken, string json)
        {
            HttpRequestMessage request = HttpServiceClient.GetRequestTemplate(method, uri, securityToken);

            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.ApplicationJsonContentType);

            return request;
        }

        private static async Task<TResponse> GetResponseFromJson<TResponse, TDTO>(
            HttpResponseMessage response, 
            JsonSerializerSettings settings,
            Func<TDTO, TResponse> convertDTO)
        {
            if (response.Content != null && response.Content.Headers.ContentLength > 0)
            {
                if (response.Content.Headers.ContentType.MediaType == Constants.ApplicationJsonContentType)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return convertDTO(JsonConvert.DeserializeObject<TDTO>(content, settings));
                }

                throw new ArgumentException("Response with invalid Content Type (" + response.Content.Headers.ContentType.MediaType + "), expected " + Constants.ApplicationJsonContentType + ".");
            }

            throw new ArgumentException("No content found from response with code " + response.StatusCode + ", expected content of Type " + typeof(TResponse) + ".");
        }

        private static Task<byte[]> GetResponseAsBytes(HttpResponseMessage response)
        {
            if (response.Content != null && response.Content.Headers.ContentLength > 0)
            {
                return response.Content.ReadAsByteArrayAsync();
            }

            throw new ArgumentException("No content found from response with code " + response.StatusCode + ".");
        }

        private static string TryGetLocationFromResponseHeader(HttpResponseMessage response)
        {
            if (response.Headers.Location != null)
            {
                return response.Headers.Location.ToString();
            }

            return null;
        }

        private static ServiceResultCode GetServiceResultCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return ServiceResultCode.Success;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.RequestTimeout:
                    return ServiceResultCode.RequestTimeout;

                case HttpStatusCode.Conflict:
                    return ServiceResultCode.Conflict;

                case HttpStatusCode.BadRequest:
                    return ServiceResultCode.BadRequest;

                case HttpStatusCode.NotFound:
                    return ServiceResultCode.ResourceNotFound;

                case HttpStatusCode.Unauthorized:
                    return ServiceResultCode.Unauthorized;

                case HttpStatusCode.InternalServerError:
                    return ServiceResultCode.InternalServerError;

                default:
                    return ServiceResultCode.Unknown;
            }
        }

        private static string GetFailureDetails(HttpResponseMessage response)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Unsuccessful status code returned (" + (int)response.StatusCode + " - " + response.ReasonPhrase + ") for request (" + response.RequestMessage + ").");

            if (response.Content != null &&
                response.Content.Headers.ContentLength > 0 &&
                HttpServiceClient.IsTextContent(response.Content))
            {
                sb.AppendLine();
                sb.Append("Content: " + response.Content.ReadAsStringAsync().Result);
            }

            return sb.ToString();
        }

        private static HttpRequestMessage CreateGetRequest(string uri, string securityToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            HttpServiceClient.SetUserAgent(request);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            if (!string.IsNullOrWhiteSpace(securityToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.AuthenticationHeaderPrefix, securityToken);
            }

            return request;
        }

        private static HttpRequestMessage CreateFormPostRequest(string uri, string securityToken, params RequestParameter[] parameters)
        {
            HttpRequestMessage request = HttpServiceClient.GetRequestTemplate(HttpMethod.Post, uri, securityToken);
            IEnumerable<KeyValuePair<string, string>> items = parameters.Select(parameter => new KeyValuePair<string, string>(parameter.ParameterName, parameter.Value));

            request.Content = new FormUrlEncodedContent(items);

            return request;
        }

        private static HttpRequestMessage CreateMultipartPostRequest(
            string uri, 
            byte[] file, 
            string fileName,
            string securityToken,
            params RequestParameter[] parameters)
        {
            Throw.IfArgumentNull(file, "file");
            Throw.IfArgumentNullOrWhitespace(fileName, "fileName");

            HttpRequestMessage request = HttpServiceClient.GetRequestTemplate(HttpMethod.Post, uri, securityToken);

            string boundary = HttpServiceClient.CreateBoundary();
            MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);

            StreamContent streamConent = HttpServiceClient.CreateFileContent(file, fileName);
            multipartContent.Add(streamConent);

            if (parameters != null)
            {
                foreach (RequestParameter parameter in parameters)
                {
                    if (!string.IsNullOrWhiteSpace(parameter.Value))
                    {
                        multipartContent.Add(new StringContent(parameter.Value), parameter.ParameterName);
                    }
                }
            }

            request.Content = multipartContent;

            return request;
        }

        private static StreamContent CreateFileContent(byte[] file, string fileName)
        {
            StreamContent streamConent = new StreamContent(new MemoryStream(file));

            streamConent.Headers.ContentDisposition = new ContentDispositionHeaderValue(Constants.FormContentDispositionType);
            streamConent.Headers.ContentDisposition.Name = "\"" + Constants.TicketImageContentKey + "\"";
            streamConent.Headers.ContentDisposition.FileName = "\"" + fileName + "\"";

            streamConent.Headers.ContentType = new MediaTypeHeaderValue(Constants.ImageJpgContentType); 

            return streamConent;
        }

        private static HttpRequestMessage GetRequestTemplate(HttpMethod httpMethod, string uri, string securityToken)
        {
            Throw.IfArgumentNull(httpMethod, nameof(httpMethod));
            Throw.IfArgumentNullOrWhitespace(uri, nameof(uri));

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, uri);

            HttpServiceClient.SetUserAgent(request);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            if (!string.IsNullOrWhiteSpace(securityToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.AuthenticationHeaderPrefix, securityToken);
            }

            return request;
        }

        private static void SetUserAgent(HttpRequestMessage request)
        {
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Constants.UserAgent, null));
        }

        private static bool IsTextContent(HttpContent content)
        {
            return 
                content.Headers.ContentType.MediaType.StartsWith(
                    Constants.TextContentType,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildQuery(params RequestParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Constants.QueryPrefix);

            foreach (RequestParameter parameter in parameters)
            {
                sb.Append(parameter.ParameterName);
                sb.Append(Constants.ValueSeperator);
                sb.Append(parameter.Value);
                sb.Append(Constants.ArgumentSeperator);
            }

            return sb.ToString(0, sb.Length - 1);
        }

        private static string CreateBoundary()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            sb.Append(Constants.Boundary);
            for (int i = 0; i < Constants.BoundaryPadLength; i++)
            {
                int index = random.Next(Constants.BoundaryCharacters.Length);
                sb.Append(Constants.BoundaryCharacters[index]);
            }

            return sb.ToString();
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings.ContractResolver = new PrivateSetterContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.None;
            settings.Converters.Add(new CustomEnumConverter());

            return settings;
        }
    }
}