using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Represents the result of a command.
    /// </summary>
    public class CommandResult
    {
        private static class Constants
        {
            public const int FalseInt = 0;
            public const int TrueInt = 1;
        }

        private readonly ConcurrentBag<CommandExecutionEventType> events;
        private int isDone;

        /// <summary>
        /// Initializes a new instance of the CommandResult class.
        /// </summary>
        /// <param name="commandGroup">The group for the command.</param>
        /// <param name="commandName">The name of the command.</param>
        internal CommandResult(string commandGroup, string commandName)
        {
            Throw.IfArgumentNullOrWhitespace(commandGroup, "commandGroup");
            Throw.IfArgumentNullOrWhitespace(commandName, "commandName");

            this.CommandKey = new CommandKey(commandGroup, commandName);
            this.CommandGroup = commandGroup;
            this.CommandName = commandName;

            this.events = new ConcurrentBag<CommandExecutionEventType>();
        }

        /// <summary>
        /// Gets a list of events that were raised during the execution of a command.
        /// </summary>
        public IEnumerable<CommandExecutionEventType> Events
        {
            get { return this.events.ToArray(); }
        }

        /// <summary>
        /// Gets the key for the command.
        /// </summary>
        public CommandKey CommandKey { get; private set; }

        /// <summary>
        /// Gets the group the command is in.
        /// </summary>
        public string CommandGroup { get; private set; }

        /// <summary>
        /// Gets the name for the command.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the date and time the command started.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// Gets the amount of time, in milliseconds, spent executing the command.
        /// </summary>
        public int ExecutionLatency { get; private set; }

        /// <summary>
        /// Gets an exception if a failure occurred while executing the command.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets whether or not the command executed successfully.
        /// </summary>
        public bool IsSuccessful
        {
            get { return this.Exception == null; }
        }

        /// <summary>
        /// Gets whether or not the command failed to finish execution due to a time out.
        /// </summary>
        public bool DidExecutionTimeout
        {
            get { return this.events.Any(_event => _event == CommandExecutionEventType.Timeout); }
        }

        /// <summary>
        /// Sets the exception for the result if one occurred during the execution of a command.
        /// </summary>
        /// <param name="exception">An exception to set.</param>
        internal void SetException(Exception exception)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Adds an event.
        /// </summary>
        /// <param name="eventType">An event.</param>
        internal void AddEvent(CommandExecutionEventType eventType)
        {
            this.events.Add(eventType);
        }

        /// <summary>
        /// Signals that a command has been started.
        /// </summary>
        internal void SignalExecutionStarted()
        {
            this.StartDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Signals that the command is done executing regardless of the outcome.
        /// </summary>
        /// <param name="completionEventType">The event that was raised signaling that the execution is done.</param>
        internal void SignalExecutionDone(CommandExecutionEventType completionEventType)
        {
            if (Interlocked.CompareExchange(ref this.isDone, Constants.TrueInt, Constants.FalseInt) != Constants.FalseInt)
            {
                throw new InvalidOperationException("The command has already been marked as done.");
            }

            this.ExecutionLatency = (int)(DateTime.UtcNow - this.StartDateTime).TotalMilliseconds;
            this.AddEvent(completionEventType);
        }
    }

    /// <summary>
    /// Represents the result and response for a command.
    /// </summary>
    public class CommandResult<TResponse> : CommandResult
    {
        private TResponse response;

        /// <summary>
        /// Initializes a new instance of the CommandResult class.
        /// </summary>
        /// <param name="commandGroup">The group for the command.</param>
        /// <param name="commandName">The name of the command.</param>
        internal CommandResult(string commandGroup, string commandName)
            : base (commandGroup, commandName)
        {
        }

        /// <summary>
        /// Gets the response from the command. If the command failed, this will throw an exception.
        /// This is a shortcut to the response inside the ExecutionResult.
        /// </summary>
        public TResponse Response
        {
            get
            {
                if (!this.IsSuccessful)
                {
                    throw new InvalidOperationException("A response is not available, the command either failed during execution or the result of the request was not successful.");
                }

                return this.response;
            }
        }

        /// <summary>
        /// Sets the response returned from the execution of a command.
        /// </summary>
        /// <param name="response">The response from the execution of the command.</param>
        internal void SetResponse(TResponse response)
        {
            this.response = response;
        }
    }
}