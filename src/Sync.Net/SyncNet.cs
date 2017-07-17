using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net
{
    public class SyncNetProgressChangedEventArgs : EventArgs
    {
        public int TotalFiles;
        public int ProcessedFiles;
        public long TotalBytes { get; set; }
        public long ProcessedBytes { get; set; }
    }

    public delegate void SyncNetProgressChangedDelegate(SyncNet sender, SyncNetProgressChangedEventArgs e);

    public class SyncNet
    {
        private IDirectoryObject _sourceDirectory;
        private IDirectoryObject _targetDirectory;
        private IFileObject _sourceFile;
        private int _totalFiles;
        private int _processedFiles;
        private long _totalBytes;
        private long _processedBytes;

        public SyncNet(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            IEnumerable<IFileObject> files = sourceDirectory.GetFiles(true);
            _totalFiles = files.Count();
            foreach (var fileObject in files)
            {
                _totalBytes += fileObject.Size;
            }
        }

        public SyncNet(IFileObject sourceFile, IDirectoryObject targetDirectory)
        {
            _sourceFile = sourceFile;
            _targetDirectory = targetDirectory;
        }

        private void Backup(IFileObject file, IDirectoryObject targetDirectory)
        {
            IFileObject targetFile = targetDirectory.GetFile(file.Name);
            if (!targetFile.Exists)
            {
                targetFile.Create();
            }

            var uploadedBytes = (long)0;
            using (var stream = file.GetStream())
            {
                uploadedBytes += stream.Length;
                using (var destination = targetFile.GetStream())
                {
                    stream.CopyTo(destination);
                }
            }

            UpdateProgess(uploadedBytes);
        }

        private void UpdateProgess(long bytesUploaded)
        {
            _processedFiles++;
            _processedBytes += bytesUploaded;
            OnProgressChanged(
                new SyncNetProgressChangedEventArgs
                {
                    ProcessedFiles = _processedFiles,
                    TotalFiles = _totalFiles,
                    ProcessedBytes = _processedBytes,
                    TotalBytes = _totalBytes
                });
        }

        public void Backup()
        {
            if(_sourceDirectory != null)
                Backup(_sourceDirectory, _targetDirectory);
            else
            {
                Backup(_sourceFile, _targetDirectory);
            }
        }

        private void Backup(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            var files = sourceDirectory.GetFiles();
            foreach (var fileObject in files)
            {
                Backup(fileObject, targetDirectory);
            }

            var subDirectories = sourceDirectory.GetDirectories();
            foreach (var subDirectory in subDirectories)
            {
                var targetSubDirectory = targetDirectory.GetDirectory(subDirectory.Name);
                if (!targetSubDirectory.Exists)
                    targetSubDirectory.Create();

                Backup(subDirectory, targetSubDirectory);
            }
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        protected virtual void OnProgressChanged(SyncNetProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
