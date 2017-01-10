using System;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Defines an object that provides visual progress updates.
    /// </summary>
    public interface IProgress
    {
        /// <summary>
        /// Occurs when the progress description has changed.
        /// </summary>
        event Action DescriptionChanged;

        /// <summary>
        /// Occurs when the progress value has changed.
        /// </summary>
        event Action ValueChanged;

        /// <summary>
        /// Occurs when the maximum value has changed.
        /// </summary>
        event Action MaximumChanged;

        /// <summary>
        /// Gets the description for the progress.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        int Value { get; }

        /// <summary>
        /// Gets the maximum value for the progress.
        /// </summary>
        int Maximum { get; }
    }
}