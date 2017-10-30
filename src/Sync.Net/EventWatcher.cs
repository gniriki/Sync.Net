using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net
{
    public class EventWatcher
    {
        private readonly ProcessorConfiguration _configuration;
        private readonly IFileWatcher _fileWatcher;
        private readonly IProcessor _processor;

        public EventWatcher(IProcessor processor, IConfigurationProvider configurationProvider,
            IFileWatcher watcher)
        {
            _configuration = configurationProvider.Current;
            _fileWatcher = watcher;

            _processor = processor;
        }

        public void Start()
        {
            _fileWatcher.Created += (sender, args) =>
            {
                if (_fileWatcher.IsDirectory(args.FullPath))
                {
                    StaticLogger.Log($"Directory created: {args.FullPath}, processing...");
                    var directory = new LocalDirectoryObject(args.FullPath);
                    _processor.ProcessDirectory(directory);
                }
                else
                {
                    StaticLogger.Log($"File created: {args.FullPath}, processing...");
                    var file = new LocalFileObject(args.FullPath);
                    _processor.CopyFile(file);
                }
            };

            _fileWatcher.Renamed += (sender, args) =>
            {
                _processor.RenameFile(new LocalFileObject(args.OldFullPath), args.Name);
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }

        private void _fileWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}