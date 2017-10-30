using System;

namespace Sync.Net.Processing
{
    public abstract class TaskQueue : ITaskQueue
    {
        public abstract void Queue(ITask task);
        public abstract int Count { get; }

        public event TaskQueueDelegate TaskStarting;
        public event TaskQueueDelegate TaskCompleted;
        public event TaskQueueErrorDelegate TaskError;

        protected virtual void OnTaskCompleted(ITask task)
        {
            TaskCompleted?.Invoke(new TaskQueueEventArgs(task));
        }

        protected virtual void OnTaskStarting(ITask task)
        {
            TaskStarting?.Invoke(new TaskQueueEventArgs(task));
        }

        protected virtual void OnTaskError(TaskQueueErrorEventArgs eventargs)
        {
            TaskError?.Invoke(eventargs);
        }

        public virtual void ExecuteTask(ITask task)
        {
            try
            {
                OnTaskStarting(task);
                task.Execute();
                OnTaskCompleted(task);
            }
            catch (Exception e)
            {
                var eventArgs = new TaskQueueErrorEventArgs(task, e);
                OnTaskError(eventArgs);
                if (eventArgs.TaskQueueErrorResponse == TaskQueueErrorResponse.Retry)
                {
                    RetryTask(task);
                }
            }
        }

        public virtual void RetryTask(ITask task)
        {
            ExecuteTask(task);
        }
    }
}