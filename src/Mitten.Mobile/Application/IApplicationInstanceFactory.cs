namespace Mitten.Mobile.Application
{
    /// <summary>
    /// A factory for creating application instances.
    /// </summary>
    public interface IApplicationInstanceFactory
    {
        /// <summary>
        /// Creates a new application instance for the specified user session.
        /// </summary>
        /// <param name="host">The parent host for the application instance.</param>
        /// <param name="session">A user session.</param>
        ApplicationInstance Create(ApplicationHost host, Session session);
    }
}