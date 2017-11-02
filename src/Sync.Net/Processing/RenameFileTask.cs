using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class RenameFileTask : ITask
    {
        public IFileObject File { get; }
        public string NewName { get; }

        public RenameFileTask(IFileObject file, string newName)
        {
            File = file;
            NewName = newName;
        }

        public void Execute()
        {
            File.Rename(NewName);
            OnTaskProgress(new TaskProgressEventArgs(1,1));
        }

        public Task ExecuteAsync()
        {
            return Task.Run(() => Execute());
        }

        public event TaskProgressEventHandler TaskProgress;

        public override string ToString()
        {
            return $"Rename file {File.FullName} to {NewName}";
        }

        protected virtual void OnTaskProgress(TaskProgressEventArgs eventargs)
        {
            TaskProgress?.Invoke(eventargs);
        }
    }
}
