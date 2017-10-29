using System.Collections.Generic;

namespace Sync.Net.Processing
{
    public class ManualTaskQueue : TaskQueue
    {
        private readonly Queue<ITask> _queue = new Queue<ITask>();

        public override void Queue(ITask task)
        {
            _queue.Enqueue(task);
        }

        public void ExecuteNextTask()
        {
            Execute(_queue.Dequeue());
        }

        private void Execute(ITask task)
        {
            OnTaskStarting(task);
            task.Execute();
            OnTaskCompleted(task);
        }

        public override int Count => _queue.Count;
    }
}