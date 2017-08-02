using System.IO;

namespace Sync.Net.UI.Utils
{
    public interface IFileWatcher
    {
        event FileSystemEventHandler Changed;
        event FileSystemEventHandler Created;
        event FileSystemEventHandler Deleted;
        event ErrorEventHandler Error;
        event RenamedEventHandler Renamed;
        void WatchForChanges(string path);
    }
}