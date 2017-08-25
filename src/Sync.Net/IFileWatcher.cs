using System.IO;

namespace Sync.Net
{
    public interface IFileWatcher
    {
        event FileSystemEventHandler Changed;
        event FileSystemEventHandler Created;
        event FileSystemEventHandler Deleted;
        event ErrorEventHandler Error;
        event RenamedEventHandler Renamed;
        void WatchForChanges(string path);
        bool IsDirectory(string path);
    }
}