using System;
using System.Threading.Tasks;
using Sync.Net.Configuration;

namespace Sync.Net
{
    public class SyncNetWatcher
    {
        public Action<string> Log { get; set; }
        private ISyncNetTask _task;
        private ISyncNetTaskFactory _taskFactory;
        private SyncNetConfiguration _configuration;
        private IFileWatcher _fileWatcher;

        public SyncNetWatcher(ISyncNetTaskFactory taskFactory, SyncNetConfiguration configuration,
            IFileWatcher watcher)
        {
            _configuration = configuration;
            _taskFactory = taskFactory;
            _fileWatcher = watcher;

            _task = _taskFactory.Create(_configuration);
            _task.ProgressChanged += Task_ProgressChanged;
        }

        public void Watch()
        {
            _fileWatcher.Created += (sender, args) =>
            {
                var realivePath = args.FullPath.Substring(_configuration.LocalDirectory.Length);
                WriteToLog($"File created: {realivePath}, uploading.");
                _task.UpdateFile(realivePath);
                WriteToLog($"Done uploading {realivePath}");
            };

            _fileWatcher.WatchForChanges(_configuration.LocalDirectory);
        }

        public async Task Sync()
        {
            WriteToLog("Preparing...");
            await Task.Run(() => _task.Run());
            WriteToLog("Finished!");
        }

        private void Task_ProgressChanged(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs args)
        {
            WriteToLog($"Uploaded {args.CurrentFile.Name}. {args.ProcessedFiles}/{args.TotalFiles} processed.\n");
        }

        private void WriteToLog(string line)
        {
            Log?.Invoke(line);
        }
    }
}