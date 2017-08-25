using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net
{
    public class SyncNetWatcher
    {
        private readonly SyncNetConfiguration _configuration;
        private readonly IFileWatcher _fileWatcher;
        private readonly ISyncNetTask _task;

        public SyncNetWatcher(ISyncNetTask task, SyncNetConfiguration configuration,
            IFileWatcher watcher)
        {
            _configuration = configuration;
            _fileWatcher = watcher;

            _task = task;
        }

        public void Watch()
        {
            _fileWatcher.Created += (sender, args) =>
            {
                if (_fileWatcher.IsDirectory(args.FullPath))
                {
                    StaticLogger.Log($"Directory created: {args.FullPath}, processing...");
                    var directory = new LocalDirectoryObject(args.FullPath);
                    _task.ProcessDirectoryAsync(directory);
                }
                else
                {
                    StaticLogger.Log($"File created: {args.FullPath}, processing...");
                    var file = new LocalFileObject(args.FullPath);
                    _task.ProcessFileAsync(file);
                }
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }
    }
}