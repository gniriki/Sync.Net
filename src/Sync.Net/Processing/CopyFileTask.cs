using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class CopyFileTask : ITask
    {
        private IFileObject _file;
        private IDirectoryObject _targetDirectory;

        public CopyFileTask(IFileObject file, IDirectoryObject targetDirectory)
        {
            _file = file;
            _targetDirectory = targetDirectory;
        }

        public void Execute()
        {
            if(!_file.Exists)
                return;
            
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
