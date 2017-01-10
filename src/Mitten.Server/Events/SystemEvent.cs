namespace Mitten.Server.Events
{
    /// <summary>
    /// Base class for all system events. Events that broadcast information about the 
    /// health or state of the system should inherit from this class.
    /// </summary>
    public abstract class SystemEvent : EventData
    {
        /// <summary>
        /// Initializes a new instance of the SystemEvent class.
        /// </summary>
        protected SystemEvent()
            : base(null, EventType.System)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SystemEvent class.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        protected SystemEvent(string name)
            : base(name, EventType.System)
        {
        }
    }
}