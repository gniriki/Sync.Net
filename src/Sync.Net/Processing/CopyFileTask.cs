using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class CopyFileTask : ITask
    {
        private IFileObject _file;
        private IDirectoryObject _targetDirectory;
        private TimeSpan _timeout = new TimeSpan(0, 0, 5);

        public CopyFileTask(IFileObject file, IDirectoryObject targetDirectory)
        {
            _file = file;
            _targetDirectory = targetDirectory;
        }

        public CopyFileTask(IFileObject file, IDirectoryObject targetDirectory, TimeSpan timeout)
            : this(file, targetDirectory)
        {
            _timeout = timeout;
        }

        public void Execute()
        {
            if(!_file.Exists)
                return;

            WaitForFileToBeReady(_file);

            if (!_targetDirectory.Exists)
                _targetDirectory.Create();

            var targetFile = _targetDirectory.GetFile(_file.Name);
            if (!targetFile.Exists || _file.ModifiedDate >= targetFile.ModifiedDate)
            {
                if (!targetFile.Exists)
                    targetFile.Create();

                using (var stream = _file.GetStream())
                {
                    var totalBytes = stream.Length;
                    using (var destination = targetFile.GetStream())
                    {
                        long bufferSize = 10 * 1024 * 1024;
                        byte[] buffer = new byte[bufferSize];
                        int progress = 0;
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destination.Write(buffer, 0, read);
                            destination.Flush();
                            progress += read;
                            OnTaskProgress(new TaskProgressEventArgs(totalBytes, progress));
                        }
                    }
                }

                targetFile.ModifiedDate = _file.ModifiedDate;
            }
        }

        private void WaitForFileToBeReady(IFileObject file)
        {
            long start = DateTime.Now.Ticks;
            while (true)
            {
                if (file.IsReady)
                    break;

                Thread.SpinWait(100);
                long now = DateTime.Now.Ticks;
                if (now - start > _timeout.Ticks)
                    throw new TimeoutException(
                        $"File {file} has been locked for more than {_timeout.Seconds} seconds.");
            }
        }

        public Task ExecuteAsync()
        {
            return Task.Run(() => Execute());
        }

        public event TaskProgressEventHandler TaskProgress;

        public IFileObject File => _file;

        public override string ToString()
        {
            return $"Copy file {_file.FullName} to {_targetDirectory.FullName}";
        }

        protected virtual void OnTaskProgress(TaskProgressEventArgs eventargs)
        {
            TaskProgress?.Invoke(eventargs);
        }
    }
}
