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
                    using (var destination = targetFile.GetStream())
                    {
                        stream.CopyTo(destination);
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

        public IFileObject File => _file;

        public override string ToString()
        {
            return $"Copy file {_file.FullName} to {_targetDirectory.FullName}";
        }
    }
}
