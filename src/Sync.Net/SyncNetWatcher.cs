using Sync.Net.Configuration;

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
                    _task.ProcessDirectoryAsync(args.FullPath);
                }
                else
                {
                    StaticLogger.Log($"File created: {args.FullPath}, processing...");
                    _task.ProcessFileAsync(args.FullPath);
                }
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }
    }
}