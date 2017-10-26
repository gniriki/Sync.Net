using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net
{
    public class SyncNetWatcher
    {
        private readonly SyncNetConfiguration _configuration;
        private readonly IFileWatcher _fileWatcher;
        private readonly IProcessor _processor;

        public SyncNetWatcher(IProcessor processor, IConfigurationProvider configurationProvider,
            IFileWatcher watcher)
        {
            _configuration = configurationProvider.Current;
            _fileWatcher = watcher;

            _processor = processor;
        }

        public void Watch()
        {
            _fileWatcher.Created += (sender, args) =>
            {
                if (_fileWatcher.IsDirectory(args.FullPath))
                {
                    StaticLogger.Log($"Directory created: {args.FullPath}, processing...");
                    var directory = new LocalDirectoryObject(args.FullPath);
                    _processor.ProcessDirectoryAsync(directory);
                }
                else
                {
                    StaticLogger.Log($"File created: {args.FullPath}, processing...");
                    var file = new LocalFileObject(args.FullPath);
                    _processor.ProcessFileAsync(file);
                }
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }
    }
}