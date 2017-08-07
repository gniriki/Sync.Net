using System.IO;

namespace Sync.Net.UI.Utils
{
    public class FileWatcher : IFileWatcher
    {
        private readonly FileSystemWatcher _watcher;

        public FileWatcher()
        {
            _watcher = new FileSystemWatcher();
        }

        public event FileSystemEventHandler Changed
        {
            add { _watcher.Changed += value; }
            remove { _watcher.Changed -= value; }
        }

        public event FileSystemEventHandler Created
        {
            add { _watcher.Created += value; }
            remove { _watcher.Created -= value; }
        }

        public event FileSystemEventHandler Deleted
        {
            add { _watcher.Deleted += value; }
            remove { _watcher.Deleted -= value; }
        }

        public event ErrorEventHandler Error
        {
            add { _watcher.Error += value; }
            remove { _watcher.Error -= value; }
        }

        public event RenamedEventHandler Renamed
        {
            add { _watcher.Renamed += value; }
            remove { _watcher.Renamed -= value; }
        }

        public void WatchForChanges(string path)
        {
            _watcher.Path = path;
            _watcher.EnableRaisingEvents = true;
        }
    }
}