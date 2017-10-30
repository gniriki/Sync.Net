namespace Sync.Net.Processing
{
    public delegate void TaskQueueDelegate(TaskQueueEventArgs eventArgs);
    public delegate void TaskQueueErrorDelegate(TaskQueueErrorEventArgs eventArgs);

    public interface ITaskQueue
    {
        void Queue(ITask task);
        event TaskQueueDelegate TaskStarting;
        event TaskQueueDelegate TaskCompleted;
        int Count { get; }
        event TaskQueueErrorDelegate TaskError;
    }
}