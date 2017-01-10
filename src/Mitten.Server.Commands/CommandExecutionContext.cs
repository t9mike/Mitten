using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mitten.Server.Events;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Encapsulates an environment for executing and managing commands.
    /// </summary>
    public class CommandExecutionContext
    {
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Initializes a new instance of the CommandExecutionContext class.
        /// </summary>
        public CommandExecutionContext()
            : this (NoOpEventPublisher.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandExecutionContext class.
        /// </summary>
        /// <param name="eventPublisher">An event publisher used to publish events raised by this execution context.</param>
        public CommandExecutionContext(IEventPublisher eventPublisher)
        {
            Throw.IfArgumentNull(eventPublisher, "eventPublisher");
            this.eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Executes an action as a command using the current context's scheduler and default group/properties.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">An action that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult> Execute(string commandName, Action executeCommand)
        {
            return this.Execute("Default", commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes an action as a command using the current context's scheduler and default properties.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">An action that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult> Execute(string groupName, string commandName, Action executeCommand)
        {
            return this.Execute(groupName, commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes an action as a command using the current context's scheduler.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        /// <param name="executeCommand">An action that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult> Execute(string groupName, string commandName, CommandProperties commandProperties, Action executeCommand)
        {
            return
                this.Execute(
                    new DelegateCommand<Unit>(
                        groupName,
                        commandName,
                        commandProperties,
                        () =>
                        {
                            executeCommand();
                            return null;
                        }))
                .ContinueWith<CommandResult>(task => task.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler and default group/properties.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string commandName, Func<TResponse> executeCommand)
        {
            return this.Execute("Default", commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler and default properties.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string groupName, string commandName, Func<TResponse> executeCommand)
        {
            return this.Execute(groupName, commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string groupName, string commandName, CommandProperties commandProperties, Func<TResponse> executeCommand)
        {
            return this.Execute(new DelegateCommand<TResponse>(groupName, commandName, commandProperties, executeCommand));
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler and default group/properties.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string commandName, Func<Task<TResponse>> executeCommand)
        {
            return this.Execute("Default", commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler and default properties.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string groupName, string commandName, Func<Task<TResponse>> executeCommand)
        {
            return this.Execute(groupName, commandName, CommandProperties.Default, executeCommand);
        }

        /// <summary>
        /// Executes a function as a command using the current context's scheduler.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        /// <param name="executeCommand">A function that will be executed as command.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(string groupName, string commandName, CommandProperties commandProperties, Func<Task<TResponse>> executeCommand)
        {
            return this.Execute(new AsyncDelegateCommand<TResponse>(groupName, commandName, commandProperties, executeCommand));
        }

        /// <summary>
        /// Executes the command based on the current context's scheduler.
        /// </summary>
        /// <param name="command">A command to execute.</param>
        /// <returns>A task encapsulating the executing command.</returns>
        public Task<CommandResult<TResponse>> Execute<TResponse>(BaseCommand<TResponse> command)
        {
            CommandResult<TResponse> commandResult = new CommandResult<TResponse>(command.GroupName, command.CommandName);
            IObservable<TResponse> observable = this.ToObservable(command, commandResult);

            return this.Execute(observable, command, commandResult);
        }

        /// <summary>
        /// Raises a warning for a command.
        /// </summary>
        /// <param name="command">The command raising the warning.</param>
        /// <param name="message">A warning message.</param>
        internal void RaiseWarning<TResponse>(BaseCommand<TResponse> command, string message)
        {
            this.eventPublisher.Publish(
                new CommandWarningEvent(
                    command.CommandKey,
                    command.GroupName,
                    command.CommandName, 
                    message));
        }

        private IObservable<TResponse> ToObservable<TResponse>(BaseCommand<TResponse> command, CommandResult<TResponse> commandResult)
        {
            if (!command.AcquireForExecution(this))
            {
                throw new InvalidOperationException("The command has already been acquired for execution, if you are trying to execute it again, you must instantiate a new instance of the command.");
            }

            IObservable<TResponse> observable = command.GetExecutionObservable();

            observable = this.WithMetrics(command, commandResult, observable);
            observable = this.WithScheduling(observable);
            observable = this.WithTimeout(command, observable);

            return this.WithResultHandling(command, commandResult, observable);
        }

        private IObservable<TResponse> WithResultHandling<TResponse>(
            BaseCommand<TResponse> command, 
            CommandResult<TResponse> commandResult, 
            IObservable<TResponse> observable)
        {
            return 
                observable.Do(
                    commandResult.SetResponse,
                    exception => this.OnExecutionException(command, commandResult, exception),
                    () => this.ExcecutionDone(command, commandResult, CommandExecutionEventType.Success));
        }

        private IObservable<TResponse> WithMetrics<TResponse>(
            BaseCommand<TResponse> command, 
            CommandResult<TResponse> commandResult, 
            IObservable<TResponse> observable)
        {
            return
                Observable.Create<TResponse>(
                    observer =>
                    {
                        commandResult.SignalExecutionStarted();
                        this.eventPublisher.Publish(new CommandExecutionStartedEvent(command.CommandKey));

                        observable.Subscribe(observer);

                        return Disposable.Empty;
                    });
        }

        private IObservable<TResponse> WithScheduling<TResponse>(IObservable<TResponse> observable)
        {
            return observable.SubscribeOn(Scheduler.Default);
        }

        private IObservable<TResponse> WithTimeout<TResponse>(BaseCommand<TResponse> command, IObservable<TResponse> observable)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(command.CommandProperties.ExecutionTimeoutMilliseconds);
            return observable.Timeout(timeout);
        }

        private void OnExecutionException<TResponse>(BaseCommand<TResponse> command, CommandResult<TResponse> commandResult, Exception exception)
        {
            CommandExecutionEventType eventType;

            if (exception is BadRequestException)
            {
                commandResult.SetException(exception);
                eventType = CommandExecutionEventType.BadRequest;
            }
            else if (exception is TimeoutException)
            {
                commandResult.SetException(
                    new CommandExecutionException(
                        CommandFailureType.Timeout,
                        "Command (group: " + command.GroupName + " - name: " + command.CommandName + ") timed out.",
                        exception));

                eventType = CommandExecutionEventType.Timeout;
            }
            else if (exception is UnhandledCommandException)
            {
                commandResult.SetException(
                    new CommandExecutionException(
                        CommandFailureType.CommandException,
                        "Command (group: " + command.GroupName + " - name: " + command.CommandName + ") failed during execution.",
                        exception.InnerException));

                eventType = CommandExecutionEventType.CommandException;
            }
            else
            {
                commandResult.SetException(exception);
                eventType = CommandExecutionEventType.InternalFailure;
            }

            this.ExcecutionDone(command, commandResult, eventType);
        }

        private void ExcecutionDone<TResponse>(BaseCommand<TResponse> command, CommandResult<TResponse> commandResult, CommandExecutionEventType eventType)
        {
            commandResult.SignalExecutionDone(eventType);
            this.eventPublisher.Publish(new CommandExecutedEvent(command.CommandKey, commandResult));
        }

        private Task<CommandResult<TResponse>> Execute<TResponse>(IObservable<TResponse> observable, BaseCommand<TResponse> command, CommandResult<TResponse> commandResult)
        {
            TaskCompletionSource<CommandResult<TResponse>> tcs = new TaskCompletionSource<CommandResult<TResponse>>();

            try
            {
                AnonymousObserver<TResponse> observer = 
                    new AnonymousObserver<TResponse>(
                        _ => { },
                        _ => tcs.TrySetResult(commandResult),
                        () => tcs.TrySetResult(commandResult));

                observable.Subscribe(observer);
            }
            catch (Exception ex)
            {
                this.OnExecutionException(command, commandResult, ex);
                tcs.TrySetResult(commandResult);
            }

            return tcs.Task;
        }
    }
}