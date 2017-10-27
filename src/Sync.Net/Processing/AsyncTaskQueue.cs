using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sync.Net.Processing
{
    public interface ITaskQueue
    {
        void Queue(ITask task);
    }

    public class AsyncTaskQueue : ITaskQueue
    {
        private BlockingCollection<ITask> _tasks = new BlockingCollection<ITask>();
        private bool _cancelled;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Queue(ITask task)
        {
            _tasks.Add(task);
        }

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
