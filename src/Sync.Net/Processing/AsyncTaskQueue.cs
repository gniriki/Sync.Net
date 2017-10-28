using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    public delegate void TaskQueueDelegate(TaskQueueEventArgs eventArgs);

    public interface ITaskQueue
    {
        void Queue(ITask task);
        event TaskQueueDelegate TaskStarting;
        event TaskQueueDelegate TaskCompleted;
        int Count { get; }
    }

    public abstract class TaskQueue : ITaskQueue
    {
        public abstract void Queue(ITask task);
        public abstract int Count { get; }

        public event TaskQueueDelegate TaskStarting;
        public event TaskQueueDelegate TaskCompleted;


        protected virtual void OnTaskCompleted(ITask task)
        {
            TaskCompleted?.Invoke(new TaskQueueEventArgs(task));
        }

        protected virtual void OnTaskStarting(ITask task)
        {
            TaskStarting?.Invoke(new TaskQueueEventArgs(task));
        }
    }

    public class AsyncTaskQueue : TaskQueue
    {
        private BlockingCollection<ITask> _tasks = new BlockingCollection<ITask>();
        private bool _cancelled;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public override void Queue(ITask task)
        {
            OnTaskStarting(task);
            _tasks.Add(task);
            OnTaskCompleted(task);
        }

        public override int Count => _tasks.Count;

        public void StartProcessing()
        {
            Task.Run(() =>
            {
                while (!_cancelled)
                {
                    try
                    {
                        var task = _tasks.Take(_cancellationTokenSource.Token);
                        task.Execute();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            });
        }

        public void StopProcessing()
        {
            _cancellationTokenSource.Cancel();
            _cancelled = true;
        }
    }
}
