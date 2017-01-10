namespace Mitten.Mobile.System
{
    /// <summary>
    /// Defines the runtime environment identifying where the application is currently executing.
    /// </summary>
    public enum RuntimeEnvironment
    {
        /// <summary>
        /// Invalid, do not use.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Represents a physical mobile device.
        /// </summary>
        Device,

        /// <summary>
        /// Represents a mobile device simulator.
        /// </summary>
        Simulator,
    }
}