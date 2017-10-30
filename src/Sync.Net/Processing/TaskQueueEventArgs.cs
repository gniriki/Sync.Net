using System;

namespace Sync.Net.Processing
{
    public class TaskQueueEventArgs : EventArgs
    {
        public TaskQueueEventArgs(ITask task)
        {
            Task = task;
        }

        public ITask Task { get; set; }
    }
}