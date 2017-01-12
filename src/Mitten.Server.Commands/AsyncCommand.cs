using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents a command encapsulating asynchronous logic. This command is intended for logic that
    /// invokes third party services or relies on some other asynchronous I/O.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
    public abstract class AsyncCommand<TResponse> : BaseCommand<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the AsyncCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        protected AsyncCommand(string groupName)
            : this(groupName, null, CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        protected AsyncCommand(string groupName, string commandName)
            : this(groupName, commandName, CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        protected AsyncCommand(string groupName, CommandProperties commandProperties)
            : this(groupName, null, commandProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        protected AsyncCommand(string groupName, string commandName, CommandProperties commandProperties)
            : base(groupName, commandName, commandProperties)
        {
        }

        /// <summary>
        /// Runs the command logic asynchronously and returns a Task.
        /// </summary>
        /// <returns>The Task from the command.</returns>
        protected abstract Task<TResponse> RunAsync();

        /// <summary>
        /// Gets an observable to be executed for the current command.
        /// </summary>
        /// <returns>An observable.</returns>
        internal protected override sealed IObservable<TResponse> GetExecutionObservable()
        {
            return 
                Observable.Create<TResponse>(
                    observer =>
                        Observable.FromAsync(this.RunAsync).Subscribe(
                            observer.OnNext,
                            ex => this.OnError(observer, ex),
                            observer.OnCompleted));
        }

        private void OnError(IObserver<TResponse> observer, Exception ex)
        {
            if (ex is BadRequestException)
            {
                observer.OnError(ex);
            }
            else
            {
                observer.OnError(new UnhandledCommandException(ex));
            }
        }
    }
}
