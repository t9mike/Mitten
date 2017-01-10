namespace Mitten.Server.Events
{
    /// <summary>
    /// Base class for all application events. Events that broadcast facts 
    /// about the application should inherit from this class.
    /// </summary>
    public abstract class ApplicationEvent : EventData
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationEvent class.
        /// </summary>
        protected ApplicationEvent()
            : base(null, EventType.Application)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationEvent class.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        protected ApplicationEvent(string name)
            : base(name, EventType.Application)
        {
        }
    }
}