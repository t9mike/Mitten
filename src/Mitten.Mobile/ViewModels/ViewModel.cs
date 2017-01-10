using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Mitten.Mobile.Remote;
using Mitten.Mobile.Validation;
using Mitten.Mobile.Application;
using Mitten.Mobile.Themes;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Base class for any view models.
    /// </summary>
    public abstract class ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// </summary>
        protected ViewModel()
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not the view model is performing a background load.
        /// </summary>
        public bool IsLoading
        {
            get { return this.LoadingFromServiceTask != null && !this.LoadingFromServiceTask.IsCompleted; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the view model has completed its background loading.
        /// </summary>
        public bool IsLoaded
        {
            get { return this.LoadingFromServiceTask != null && this.LoadingFromServiceTask.IsCompleted; }
        }

        /// <summary>
        /// Gets a value indicating whether the loading from service task was successful.
        /// </summary>
        public bool WasSuccessfullyLoadedFromService
        {
            get { return this.IsLoaded && this.LoadingFromServiceTask.Result.ResultCode == ServiceResultCode.Success; }
        }

        /// <summary>
        /// Gets the task responsible for any background loading of service data.
        /// </summary>
        protected Task<ServiceResult> LoadingFromServiceTask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has been initialized.
        /// </summary>
        protected bool HasBeenInitialized { get; private set; }

        /// <summary>
        /// Gets the navigation object used to navigate between screens.
        /// </summary>
        protected INavigation Navigation { get; private set; }

        /// <summary>
        /// Gets the application host.
        /// </summary>
        protected ApplicationHost ApplicationHost { get; private set; }

        /// <summary>
        /// Gets the title for a navigation bar, the default is empty.
        /// </summary>
        /// <returns>The title.</returns>
        public virtual NavigationBarTitle GetNavigationBarTitle()
        {
            return NavigationBarTitle.Empty;
        }

        /// <summary>
        /// Sets the visual style for the status bar.
        /// </summary>
        /// <param name="statusBarStyle">The status bar style.</param>
        public void SetStatusBarStyle(StatusBarStyle statusBarStyle)
        {
            this.ApplicationHost.StatusBar.SetStatusBarStyle(statusBarStyle);
        }

        /// <summary>
        /// Wraps the background loading task to show a loading overlay until it has completed.
        /// </summary>
        /// <param name="loadingOverlay">A loading over overlay to display.</param>
        /// <returns>A task representing the background loading.</returns>
        public async Task AwaitBackgroundLoading(ILoadingOverlay loadingOverlay)
        {
            using (loadingOverlay.ShowLoadingOverlay())
            {
                await this.LoadingFromServiceTask;
            }
        }

        /// <summary>
        /// Signals to the view model that the view has finished all of its loading operations.
        /// </summary>
        public void ViewFinishedLoading()
        {
            if (!this.WasSuccessfullyLoadedFromService)
            {
                this.ShowLoadingFromServiceFailedAlert(this.LoadingFromServiceTask.Result);
            }
        }

        /// <summary>
        /// Executes a command by first validating the state and then executing the command if valid.
        /// </summary>
        /// <param name="validateCommand">Validates the command prior to executing it.</param>
        /// <param name="executeCommand">Executes the command.</param>
        protected void ExecuteCommand(Func<ValidationResult> validateCommand, Action executeCommand)
        {
            ValidationResult result = validateCommand();

            if (result.HasErrors)
            {
                this.ApplicationHost.Alert.ShowAlert(result.Messages.First());
            }
            else
            {
                executeCommand();
            }
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void ExecuteServiceRequestAsync(ILoadingOverlay loadingOverlay, Func<ValidationResult> validateRequest, Func<Task<ServiceResult>> executeRequest, Action onSuccess)
        {
            this.ExecuteServiceRequestAsync(
                loadingOverlay, 
                null,
                validateRequest, 
                executeRequest, 
                onSuccess, 
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void ExecuteServiceRequestAsync(ILoadingOverlay loadingOverlay, Func<ValidationResult> validateRequest, Func<Task<ServiceResult>> executeRequest, Action<ServiceResult> onFailed)
        {
            this.ExecuteServiceRequestAsync(loadingOverlay, null, validateRequest, executeRequest, () => { }, onFailed);
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void ExecuteServiceRequestAsync(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Func<ValidationResult> validateRequest,
            Func<Task<ServiceResult>> executeRequest,
            Action onSuccess)
        {
            this.ExecuteServiceRequestAsync(
                loadingOverlay,
                progress,
                validateRequest,
                executeRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void ExecuteServiceRequestAsync(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Func<ValidationResult> validateRequest,
            Func<Task<ServiceResult>> executeRequest,
            Action onSuccess,
            Action<ServiceResult> onFailed)
        {
            Throw.IfArgumentNull(validateRequest, nameof(validateRequest));

            ValidationResult validationResult = validateRequest();

            if (!validationResult.HasErrors)
            {
                this.OnAsyncServiceRequest(loadingOverlay, progress, executeRequest(), onSuccess, onFailed);
            }
            else
            {
                this.ApplicationHost.Alert.ShowAlert(validationResult.Messages.First());
            }
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void ExecuteServiceRequestAsync<TResponse>(
            ILoadingOverlay loadingOverlay, 
            Func<ValidationResult> validateRequest, 
            Func<Task<ServiceResult<TResponse>>> executeRequest, 
            Action<TResponse> onSuccess)
        {
            this.ExecuteServiceRequestAsync(
                loadingOverlay,
                validateRequest,
                executeRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void ExecuteServiceRequestAsync<TResponse>(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Func<ValidationResult> validateRequest,
            Func<Task<ServiceResult<TResponse>>> executeRequest,
            Action<TResponse> onSuccess)
        {
            this.ExecuteServiceRequestAsync(
                loadingOverlay,
                progress,
                validateRequest,
                executeRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void ExecuteServiceRequestAsync<TResponse>(
            ILoadingOverlay loadingOverlay,
            Func<ValidationResult> validateRequest,
            Func<Task<ServiceResult<TResponse>>> executeRequest,
            Action<TResponse> onSuccess,
            Action<ServiceResult> onFailed)
        {
            this.ExecuteServiceRequestAsync(loadingOverlay, null, validateRequest, executeRequest, onSuccess, onFailed);
        }

        /// <summary>
        /// Executes an asynchronous service request if it is valid and shows a waiting overlay until the operation has completed.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="validateRequest">Validates the request prior to executing it.</param>
        /// <param name="executeRequest">Executes the asynchronous request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void ExecuteServiceRequestAsync<TResponse>(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Func<ValidationResult> validateRequest,
            Func<Task<ServiceResult<TResponse>>> executeRequest,
            Action<TResponse> onSuccess,
            Action<ServiceResult> onFailed)
        {
            Throw.IfArgumentNull(validateRequest, nameof(validateRequest));

            ValidationResult validationResult = validateRequest();

            if (!validationResult.HasErrors)
            {
                this.OnAsyncServiceRequest(loadingOverlay, progress, executeRequest(), onSuccess, onFailed);
            }
            else
            {
                this.ApplicationHost.Alert.ShowAlert(validationResult.Messages.First());
            }
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        protected void OnAsyncServiceRequest(ILoadingOverlay loadingOverlay, Task<ServiceResult> executingRequest)
        {
            this.OnAsyncServiceRequest(loadingOverlay, executingRequest, () => { });
        }

        /// <summary>
        /// Handles an executing async service request without showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest(Task<ServiceResult> executingRequest, Action onSuccess)
        {
            this.OnAsyncServiceRequest(
                new NullLoadingOverlay(), 
                null, 
                executingRequest, 
                onSuccess, 
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest(ILoadingOverlay loadingOverlay, Task<ServiceResult> executingRequest, Action onSuccess)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay, 
                null, 
                executingRequest, 
                onSuccess, 
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void OnAsyncServiceRequest(
            ILoadingOverlay loadingOverlay, 
            Task<ServiceResult> executingRequest, 
            Action onSuccess,
            Action<ServiceResult> onFailed)
        {
            this.OnAsyncServiceRequest(loadingOverlay, null, executingRequest, onSuccess, onFailed);
        }

        /// <summary>
        /// Handles an executing async service request without showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void OnAsyncServiceRequest(Task<ServiceResult> executingRequest, Action onSuccess, Action<ServiceResult> onFailed)
        {
            this.OnAsyncServiceRequest(
                new NullLoadingOverlay(),
                null,
                executingRequest,
                onSuccess,
                onFailed);
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        protected void OnAsyncServiceRequest(ILoadingOverlay loadingOverlay, IProgress progress, Task<ServiceResult> executingRequest)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                progress,
                executingRequest,
                () => { },
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest(ILoadingOverlay loadingOverlay, LongRunningTask executingRequest, Action onSuccess)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                executingRequest.Progress,
                executingRequest.Task,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest(ILoadingOverlay loadingOverlay, IProgress progress, Task<ServiceResult> executingRequest, Action onSuccess)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                progress,
                executingRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void OnAsyncServiceRequest(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Task<ServiceResult> executingRequest,
            Action<ServiceResult> onFailed)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                progress,
                executingRequest,
                () => { },
                onFailed);
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected async void OnAsyncServiceRequest(
            ILoadingOverlay loadingOverlay,
            IProgress progress, 
            Task<ServiceResult> executingRequest, 
            Action onSuccess,
            Action<ServiceResult> onFailed)
        {
            Throw.IfArgumentNull(loadingOverlay, nameof(loadingOverlay));
            Throw.IfArgumentNull(executingRequest, nameof(executingRequest));
            Throw.IfArgumentNull(onSuccess, nameof(onSuccess));
            Throw.IfArgumentNull(onFailed, nameof(onFailed));

            ServiceResult serviceResult;

            if (executingRequest.IsCompleted)
            {
                serviceResult = executingRequest.Result;
            }
            else
            {
                using (loadingOverlay.ShowLoadingOverlay(progress))
                {
                    serviceResult = await executingRequest;
                }
            }

            if (serviceResult.ResultCode == ServiceResultCode.Success)
            {
                onSuccess();
            }
            else
            {
                onFailed(serviceResult);
            }
        }

        /// <summary>
        /// Handles an executing async service request without showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest<TResponse>(Task<ServiceResult<TResponse>> executingRequest, Action<TResponse> onSuccess)
        {
            this.OnAsyncServiceRequest(
                new NullLoadingOverlay(),
                null,
                executingRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request without showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void OnAsyncServiceRequest<TResponse>(Task<ServiceResult<TResponse>> executingRequest, Action<TResponse> onSuccess, Action<ServiceResult> onFailed)
        {
            this.OnAsyncServiceRequest(
                new NullLoadingOverlay(),
                null,
                executingRequest,
                onSuccess,
                onFailed);
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest<TResponse>(ILoadingOverlay loadingOverlay, Task<ServiceResult<TResponse>> executingRequest, Action<TResponse> onSuccess)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                null,
                executingRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected void OnAsyncServiceRequest<TResponse>(
            ILoadingOverlay loadingOverlay,
            Task<ServiceResult<TResponse>> executingRequest,
            Action<TResponse> onSuccess,
            Action<ServiceResult> onFailed)
        {
            this.OnAsyncServiceRequest(loadingOverlay, null, executingRequest, onSuccess, onFailed);
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        protected void OnAsyncServiceRequest<TResponse>(ILoadingOverlay loadingOverlay, IProgress progress, Task<ServiceResult<TResponse>> executingRequest, Action<TResponse> onSuccess)
        {
            this.OnAsyncServiceRequest(
                loadingOverlay,
                progress,
                executingRequest,
                onSuccess,
                result => this.ShowLoadingFromServiceFailedAlert(result));
        }

        /// <summary>
        /// Handles an executing async service request by showing a loading overlay until it completes execution.
        /// </summary>
        /// <param name="loadingOverlay">A overlay to display over the screen during the async operation.</param>
        /// <param name="progress">An optional object to provide progress for the operation the overlay represents.</param>
        /// <param name="executingRequest">The task for an executing request.</param>
        /// <param name="onSuccess">Gets invoked if the request has completed successfully.</param>
        /// <param name="onFailed">Gets invoked if the request failed, by default a standard alert message will be shown.</param>
        protected async void OnAsyncServiceRequest<TResponse>(
            ILoadingOverlay loadingOverlay,
            IProgress progress,
            Task<ServiceResult<TResponse>> executingRequest,
            Action<TResponse> onSuccess,
            Action<ServiceResult> onFailed)
        {
            Throw.IfArgumentNull(loadingOverlay, nameof(loadingOverlay));
            Throw.IfArgumentNull(executingRequest, nameof(executingRequest));
            Throw.IfArgumentNull(onSuccess, nameof(onSuccess));
            Throw.IfArgumentNull(onFailed, nameof(onFailed));

            ServiceResult<TResponse> serviceResult;

            if (executingRequest.IsCompleted)
            {
                serviceResult = executingRequest.Result;
            }
            else
            {
                using (loadingOverlay.ShowLoadingOverlay(progress))
                {
                    serviceResult = await executingRequest;
                }
            }

            if (serviceResult.ResultCode == ServiceResultCode.Success)
            {
                onSuccess(serviceResult.Response);
            }
            else
            {
                onFailed(serviceResult);
            }
        }

        /// <summary>
        /// Occurs right after the view has been loaded and shows an alert if the background loading from service task failed.
        /// </summary>
        /// <param name="failedResult">The result for the failed service request.</param>
        protected void ShowLoadingFromServiceFailedAlert(ServiceResult failedResult)
        {
            this.ApplicationHost.Alert.ShowAlert(this.GetAlertForFailedServiceRequest(failedResult));
        }

        /// <summary>
        /// Gets a default alert message for a failed service request.
        /// </summary>
        /// <param name="serviceResult">The service result for the failed request.</param>
        /// <returns>A message used to alert the user.</returns>
        protected string GetAlertForFailedServiceRequest(ServiceResult serviceResult)
        {
            Throw.IfArgumentNull(serviceResult, nameof(serviceResult));

            if (serviceResult.ResultCode == ServiceResultCode.Invalid)
            {
                throw new ArgumentException("Invalid service result code for result.", nameof(serviceResult));
            }

            if (serviceResult.ResultCode == ServiceResultCode.Success)
            {
                throw new ArgumentException("Service result does not indicate a failure.", nameof(serviceResult));
            }

            switch (serviceResult.ResultCode)
            {
                case ServiceResultCode.BadRequest:
                case ServiceResultCode.Conflict:
                case ServiceResultCode.ResourceNotFound:
                    // TODO: is there a better way to handle these errors? most likely a retry won't work...
                    return "An unexpected response was received from the server, please wait a moment and try again.";

                case ServiceResultCode.InvalidResponseContent:
                    // TODO: is there a better way to handle these errors? most likely a retry won't work...
                    return "An invalid response was received from the server, please wait a moment and try again.";

                case ServiceResultCode.RequestTimeout:
                    // TODO: support retry
                    return "The network connection timed out, please wait a moment and try again.";

                case ServiceResultCode.CommunicationFailure:
                case ServiceResultCode.ConnectionFailure:
                    return "Failed to communicate with the server, please check your internet connection and try again.";

                case ServiceResultCode.NetworkUnavailable:
                    return "Network is unavailable, please check your internet connection and try again.";

                case ServiceResultCode.WifiRequired:
                    return "A Wi-Fi connection is required for the operation, please connect to a Wi-Fi network and try again.";

                case ServiceResultCode.Unauthorized:
                    return "Failed to authenticate with the server, if you continue to experience this problem, force close the app and relaunch it.";

                default:
                    return "An unexpected error occurred. Please wait a moment and try again.";
            }
        }

        /// <summary>
        /// Initializes the view model. This is the overload that subclasses should override 
        /// to handle custom initialization logic and to handle the optional parameter.
        /// </summary>
        /// <param name="parameter">An optional parameter used to initialize the view model.</param>
        protected virtual void Initialize(object parameter)
        {
            if (parameter != null)
            {
                throw new ArgumentException("The parameter (" + parameter.GetType() + ") is not null and should not be ignored. Subclasses overriding this method should not call base.Initialize.");
            }
        }

        /// <summary>
        /// Utility method to ensure that the Initialize parameter is the specified expected Type.
        /// </summary>
        /// <param name="parameter">The parameter passed into the Initialize method.</param>
        /// <returns>The parameter cast to the expected Type.</returns>
        protected TValue EnsureParameter<TValue>(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter is TValue)
            {
                return (TValue)parameter;
            }

            IEnumerable collection = parameter as IEnumerable;
            object param = parameter;

            if (collection != null)
            {
                param = collection.OfType<TValue>().SingleOrDefault();
            }
                
            if (!(param is TValue))
            {
                throw new ArgumentException("The parameter is an unexpected Type of " + parameter.GetType().Name + ", expected Type " + typeof(TValue).Name + ".");
            }

            return (TValue)param;
        }

        /// <summary>
        /// Ensures that the view model has finished performing all background loading.
        /// </summary>
        protected void EnsureLoaded()
        {
            if (!this.IsLoaded)
            {
                throw new ViewModelNotLoadedException();
            }
        }

        /// <summary>
        /// Reloads the data for this view model from a mobile service in the background.
        /// </summary>
        protected void ReloadFromServiceInBackground()
        {
            this.LoadingFromServiceTask = this.LoadFromServiceInBackground();
        }

        /// <summary>
        /// Performs a service request in the background if this view model requires data from a mobile service during the loading/initialization of the view model.
        /// </summary>
        /// <returns>The loading task.</returns>
        protected virtual Task<ServiceResult> LoadFromServiceInBackground()
        {
            return Task.FromResult(ServiceResult.Success());
        }

        /// <summary>
        /// Occurs after the view model has been initialized. Note: if the VM is loading data in the background
        /// from a service, it is not guaranteed to be completed when this is invoked.
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// Sets the application instance for this view model.
        /// </summary>
        /// <param name="applicationInstance">The application instance.</param>
        internal virtual void SetApplicationInstance(ApplicationInstance applicationInstance)
        {
        }

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="applicationHost">The application host that provides global access to all of the application's components.</param>
        /// <param name="navigation">Used to navigate between screens.</param>
        /// <param name="parameter">An optional parameter used to initialize the view model.</param>
        internal void Initialize(ApplicationHost applicationHost, INavigation navigation, object parameter)
        {
            this.ApplicationHost = applicationHost;

            if (applicationHost.ApplicationInstance != null)
            {
                this.SetApplicationInstance(applicationHost.ApplicationInstance);
            }

            this.Navigation = navigation;
            this.Initialize(parameter);
            this.LoadingFromServiceTask = this.LoadFromServiceInBackground();
            this.HasBeenInitialized = true;
            this.OnInitialized();
        }

        private class NullLoadingOverlay : ILoadingOverlay, IDisposable
        {
            public void Dispose()
            {
            }

            public IDisposable ShowLoadingOverlay(IProgress progress = null)
            {
                return this;
            }
        }
    }

    /// <summary>
    /// Defines a view model used within a specific application instance.
    /// </summary>
    public abstract class ViewModel<TApplicationInstance> : ViewModel
        where TApplicationInstance : ApplicationInstance
    {
        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// </summary>
        protected ViewModel()
        {
        }

        /// <summary>
        /// Gets an object that provides global access to all of the application's components.
        /// </summary>
        protected TApplicationInstance ApplicationInstance { get; private set; }

        /// <summary>
        /// Sets the application instance for this view model.
        /// </summary>
        /// <param name="applicationInstance">The application instance.</param>
        internal override void SetApplicationInstance(ApplicationInstance applicationInstance)
        {
            if (applicationInstance == null)
            {
                throw new ArgumentNullException(nameof(applicationInstance), "Application instance cannot be null, ensure that a user has been signed-in.");
            }

            this.ApplicationInstance = (TApplicationInstance)applicationInstance;
        }
    }
}