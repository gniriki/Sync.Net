using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class RenameDirectoryTask : ITask
    {
        public IDirectoryObject Directory { get; }
        public string NewName { get; }

        public RenameDirectoryTask(IDirectoryObject directory, string newName)
        {
            Directory = directory;
            NewName = newName;
        }

        public void Execute()
        {
            Directory.Rename(NewName);
        }

        public Task ExecuteAsync()
        {
            return Task.Run(() => Execute());
        }

        public override string ToString()
        {
            return $"Rename directory {Directory.FullName} to {NewName}";
        }
    }
}