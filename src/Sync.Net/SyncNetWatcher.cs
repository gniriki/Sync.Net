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
            _fileWatcher.Created += async (sender, args) =>
            {
                var relativePath = args.FullPath.Substring(_configuration.LocalDirectory.Length);
                StaticLogger.Log($"File created: {relativePath}, processing...");
                await _task.ProcessFileAsync(relativePath);
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }
    }
}