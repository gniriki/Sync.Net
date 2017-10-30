using System;

namespace Sync.Net.Processing
{
    public enum TaskQueueErrorResponse
    {
        Retry,
        Skip,
        Abort
    }

    public class TaskQueueErrorEventArgs : EventArgs
    {
        public TaskQueueErrorEventArgs(ITask task, Exception thrownException)
        {
            Task = task;
            ThrownException = thrownException;
        }

        public Exception ThrownException { get; set; }

        public ITask Task { get; set; }

        public TaskQueueErrorResponse TaskQueueErrorResponse { get; set; }
    }
}