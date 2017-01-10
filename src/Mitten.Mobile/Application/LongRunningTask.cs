using System.Threading.Tasks;
using Mitten.Mobile.Remote;

namespace Mitten.Mobile.Application
{
    /// <summary>
    /// Represents a long running task that provides progress updates.
    /// </summary>
    public class LongRunningTask
    {
        /// <summary>
        /// Initializes a new instance of the LongRunningTask class.
        /// </summary>
        /// <param name="task">The thread task for the operation.</param>
        /// <param name="progress">The progress indicator for the task.</param>
        public LongRunningTask(Task<ServiceResult> task, IProgress progress)
        {
            Throw.IfArgumentNull(task, nameof(task));
            Throw.IfArgumentNull(progress, nameof(progress));

            this.Task = task;
            this.Progress = progress;
        }

        /// <summary>
        /// Gets the thread task for the operation.
        /// </summary>
        public Task<ServiceResult> Task { get; }

        /// <summary>
        /// Gets the progress indicator for the task.
        /// </summary>
        public IProgress Progress { get; }
    }
}