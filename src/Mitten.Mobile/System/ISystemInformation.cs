namespace Mitten.Mobile.System
{
    /// <summary>
    /// A contract for providing system information.
    /// </summary>
    public interface ISystemInformation
    {
        /// <summary>
        /// Gets the current version of the executing application.
        /// </summary>
        /// <returns>The application version.</returns>
        string GetAppVersion();

        /// <summary>
        /// Gets the current runtime environment.
        /// </summary>
        /// <returns>The current runtime environment.</returns>
        RuntimeEnvironment GetRuntimeEnvironment();
    }
}