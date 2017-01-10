using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mitten.Server.Events;

namespace Mitten.Server.Commands
{
    /// <summary>
    /// Maintains a rolling set of command execution metrics.
    /// </summary>
    public class CommandExecutionMetrics
    {
        private readonly ConcurrentDictionary<CommandKey, EventCounts> commandEventCounts;

        private bool hasRegisteredWithEventDispatcher;
        private int currentExecutionCount;
        private int warningCount;
        
        /// <summary>
        /// Initializes a new instance of the CommandExecutionMetrics class.
        /// </summary>
        public CommandExecutionMetrics()
        {
            this.commandEventCounts = new ConcurrentDictionary<CommandKey, EventCounts>();
        }

        /// <summary>
        /// Registers the current handler with the specified dispatcher.
        /// </summary>
        /// <param name="eventDispatcher">An event dispatcher.</param>
        public void RegisterWithDispatcher(EventDispatcher eventDispatcher)
        {
            if (this.hasRegisteredWithEventDispatcher)
            {
                throw new InvalidOperationException("The current command execution metrics has already registered with an event dispatcher.");
            }

            eventDispatcher.Register<CommandExecutionStartedEvent>(_ => this.OnExecutionStarted());
            eventDispatcher.Register<CommandWarningEvent>(_ => this.OnCommandWarning());
            eventDispatcher.Register<CommandExecutedEvent>(eventData => this.OnExecutionComplete(eventData.CommandResult));

            this.hasRegisteredWithEventDispatcher = true;
        }

        /// <summary>
        /// Gets the number of commands that are currently executing.
        /// </summary>
        public int CurrentExecutionCount
        {
            get { return this.currentExecutionCount; }
        }

        /// <summary>
        /// Gets the number of warnings that have been raised.
        /// </summary>
        public int WarningCount
        {
            get { return this.warningCount; }
        }

        /// <summary>
        /// Gets a snapshot of counts for a command with the specified key. 
        /// </summary>
        /// <returns>A list containing the event counts.</returns>
        public EventCountsSnapshot GetCommandEventCounts(CommandKey commandKey)
        {
            EventCounts eventCounts;

            return 
                this.commandEventCounts.TryGetValue(commandKey, out eventCounts) 
                ? eventCounts.GetSnapshot() 
                : EventCountsSnapshot.Empty();

        }

        private void OnExecutionStarted()
        {
            Interlocked.Increment(ref this.currentExecutionCount);
        }
        
        private void OnCommandWarning()
        {
            Interlocked.Increment(ref this.warningCount);
        }

        private void OnExecutionComplete(CommandResult commandResult)
        {
            Interlocked.Decrement(ref this.currentExecutionCount);

            foreach (CommandExecutionEventType eventType in commandResult.Events)
            {
                this.IncrementEventCount(commandResult.CommandKey, eventType);
            }
        }

        private void IncrementEventCount(CommandKey commandKey, CommandExecutionEventType eventType)
        {
            EventCounts count;
            if (!this.commandEventCounts.TryGetValue(commandKey, out count))
            {
                this.commandEventCounts.TryAdd(commandKey, new EventCounts());
                count = this.commandEventCounts[commandKey];
            }

            count.Increment(eventType);
        }

        /// <summary>
        /// Represents a snapshot of event counts for a specific moment in time.
        /// </summary>
        public class EventCountsSnapshot
        {
            private readonly Dictionary<CommandExecutionEventType, long> eventCounts;

            /// <summary>
            /// Initializes a new instance of the Snapshot class.
            /// </summary>
            /// <param name="currentEventCounts">A list of current event counts.</param>
            internal EventCountsSnapshot(IEnumerable<KeyValuePair<CommandExecutionEventType, long>> currentEventCounts)
            {
                this.eventCounts = new Dictionary<CommandExecutionEventType, long>();
                
                foreach (KeyValuePair<CommandExecutionEventType, long> item in currentEventCounts)
                {
                    this.eventCounts.Add(item.Key, item.Value);
                }

                this.SnapshotDate = DateTime.UtcNow;
            }

            /// <summary>
            /// Gets the date and time of the snapshot.
            /// </summary>
            public DateTime SnapshotDate { get; private set; }

            /// <summary>
            /// Gets the count for the specified execution event type.
            /// </summary>
            /// <param name="eventType">The type of event to get the count for.</param>
            /// <returns>The number of occurrences of a specific event type.</returns>
            public long GetCount(CommandExecutionEventType eventType)
            {
                long count;
                this.eventCounts.TryGetValue(eventType, out count);
                return count;
            }

            /// <summary>
            /// Gets an empty snapshot.
            /// </summary>
            /// <returns>A new empty snapshot.</returns>
            internal static EventCountsSnapshot Empty()
            {
                return new EventCountsSnapshot(Enumerable.Empty<KeyValuePair<CommandExecutionEventType, long>>());
            }
        }

        private class EventCounts
        {
            private readonly ConcurrentDictionary<CommandExecutionEventType, EventCount> eventCounts;

            /// <summary>
            /// Initializes a new instance of the EventCounts class.
            /// </summary>
            internal EventCounts()
            {
                this.eventCounts = new ConcurrentDictionary<CommandExecutionEventType, EventCount>();
            }
            
            public EventCountsSnapshot GetSnapshot()
            {
                return
                    new EventCountsSnapshot(
                        this.eventCounts.Select(item => new KeyValuePair<CommandExecutionEventType, long>(item.Key, item.Value.Count)));
            }
            
            public void Increment(CommandExecutionEventType eventType)
            {
                EventCount count;
                if (!this.eventCounts.TryGetValue(eventType, out count))
                {
                    this.eventCounts.TryAdd(eventType, new EventCount());
                    count = this.eventCounts[eventType];
                }

                count.Increment();
            }
        }

        private class EventCount
        {
            private long count;

            public long Count
            {
                get { return this.count; }
            }

            public void Increment()
            {
                Interlocked.Increment(ref this.count);
            }
        }
    }
}
