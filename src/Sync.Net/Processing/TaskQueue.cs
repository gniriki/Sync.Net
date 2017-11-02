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
        public event TaskProgressEventHandler TaskProgress;

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
                task.TaskProgress += Task_TaskProgress;
                OnTaskStarting(task);
                task.Execute();
                OnTaskCompleted(task);
                task.TaskProgress -= Task_TaskProgress;
            }
            catch (Exception e)
            {
                task.TaskProgress -= Task_TaskProgress;
                var eventArgs = new TaskQueueErrorEventArgs(task, e);
                OnTaskError(eventArgs);
                if (eventArgs.TaskQueueErrorResponse == TaskQueueErrorResponse.Retry)
                {
                    RetryTask(task);
                }
            }
        }

        private void Task_TaskProgress(TaskProgressEventArgs eventArgs)
        {
            OnTaskProgress(eventArgs);
        }

        public virtual void RetryTask(ITask task)
        {
            ExecuteTask(task);
        }

        protected virtual void OnTaskProgress(TaskProgressEventArgs eventargs)
        {
            TaskProgress?.Invoke(eventargs);
        }
    }
}