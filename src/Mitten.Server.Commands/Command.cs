using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents synchronous logic meant to be executing in a controlled context.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
    /// <remarks>
    /// The command logic is intended to be implemented synchronously but maybe executed asynchronously
    /// from other executing commands. The execution policy is controlled by the execution context.
    /// </remarks>
    public abstract class Command<TResponse> : BaseCommand<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        protected Command(string groupName)
            : this(groupName, null, CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        protected Command(string groupName, string commandName)
            : this(groupName, commandName, CommandProperties.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        protected Command(string groupName, CommandProperties commandProperties)
            : this(groupName, null, commandProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        /// <param name="groupName">A name used to identify a group of similar or related commands.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandProperties">Defines the properties for this command.</param>
        protected Command(string groupName, string commandName, CommandProperties commandProperties)
            : base(groupName, commandName, commandProperties)
        {
        }

        /// <summary>
        /// Runs the command logic and returns a response.
        /// </summary>
        /// <returns>The response from the command.</returns>
        protected abstract TResponse Run();

        /// <summary>
        /// Gets an observable to be executed for the current command.
        /// </summary>
        /// <returns>An observable.</returns>
        internal protected override sealed IObservable<TResponse> GetExecutionObservable()
        {
            return
                Observable.Create<TResponse>(
                    observer =>
                    {
                        try
                        {
                            observer.OnNext(this.Run());
                            observer.OnCompleted();
                        }
                        catch (BadRequestException ex)
                        {
                            observer.OnError(ex);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(new UnhandledCommandException(ex));
                        }

                        return Disposable.Empty;
                    });
        }
    }
}