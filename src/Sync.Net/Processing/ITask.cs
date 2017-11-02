using System;
using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class TaskProgressEventArgs : EventArgs
    {
        public TaskProgressEventArgs(long maxValue, long currentValue)
        {
            MaxValue = maxValue;
            CurrentValue = currentValue;
        }

        public long MaxValue { get; set; }
        public long CurrentValue { get; set; }
        public float Percentage => CurrentValue / (float)MaxValue;
    }

    public delegate void TaskProgressEventHandler(TaskProgressEventArgs eventArgs);

    public interface ITask
    {
        void Execute();
        Task ExecuteAsync();

        event TaskProgressEventHandler TaskProgress;

        //IFileObject File { get; }
    }
}