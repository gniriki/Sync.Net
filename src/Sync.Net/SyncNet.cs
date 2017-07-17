using System;
using System.Collections.Generic;
using System.Linq;
using Sync.Net.IO;

namespace Sync.Net
{
    public class SyncNetProgressChangedEventArgs : EventArgs
    {
        public int ProcessedFiles;
        public int TotalFiles;
        public long TotalBytes { get; set; }
        public long ProcessedBytes { get; set; }
        public IFileObject CurrentFile { get; set; }
    }

    public delegate void SyncNetProgressChangedDelegate(SyncNet sender, SyncNetProgressChangedEventArgs e);

    public class SyncNet
    {
        private long _processedBytes;
        private int _processedFiles;
        private readonly IDirectoryObject _sourceDirectory;
        private readonly IFileObject _sourceFile;
        private readonly IDirectoryObject _targetDirectory;
        private readonly long _totalBytes;
        private readonly int _totalFiles;

        public SyncNet(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            var files = GetFilesToUpload(sourceDirectory, targetDirectory);
            _totalFiles = files.Count();
            foreach (var fileObject in files)
                _totalBytes += fileObject.Size;
        }

        public SyncNet(IFileObject sourceFile, IDirectoryObject targetDirectory)
        {
            _sourceFile = sourceFile;
            _targetDirectory = targetDirectory;
        }

        private static IEnumerable<IFileObject> GetFilesToUpload(IDirectoryObject source, IDirectoryObject target)
        {
            var filesToUpload = new List<IFileObject>();
            var sourceFiles = source.GetFiles();

            foreach (var sourceFile in sourceFiles)
            {
                var targetFile = target.GetFile(sourceFile.Name);
                if (!targetFile.Exists || sourceFile.ModifiedDate >= targetFile.ModifiedDate)
                    filesToUpload.Add(sourceFile);
            }

            var subDirectories = source.GetDirectories();
            foreach (var sourceSubDirectory in subDirectories)
            {
                var targetSubDirectory = target.GetDirectory(sourceSubDirectory.Name);
                filesToUpload.AddRange(GetFilesToUpload(sourceSubDirectory, targetSubDirectory));
            }

            return filesToUpload;
        }

        private void Backup(IFileObject file, IDirectoryObject targetDirectory)
        {
            var targetFile = targetDirectory.GetFile(file.Name);
            if (!targetFile.Exists || file.ModifiedDate >= targetFile.ModifiedDate)
            {
                if (!targetFile.Exists)
                    targetFile.Create();

                var uploadedBytes = (long) 0;
                using (var stream = file.GetStream())
                {
                    uploadedBytes += stream.Length;
                    using (var destination = targetFile.GetStream())
                    {
                        stream.CopyTo(destination);
                    }
                }

                targetFile.ModifiedDate = file.ModifiedDate;
                UpdateProgess(file, uploadedBytes);
            }
        }

        private void UpdateProgess(IFileObject currentFile, long bytesUploaded)
        {
            _processedFiles++;
            _processedBytes += bytesUploaded;
            OnProgressChanged(
                new SyncNetProgressChangedEventArgs
                {
                    ProcessedFiles = _processedFiles,
                    TotalFiles = _totalFiles,
                    ProcessedBytes = _processedBytes,
                    TotalBytes = _totalBytes,
                    CurrentFile = currentFile
                });
        }

        public void Backup()
        {
            if (_sourceDirectory != null)
                Backup(_sourceDirectory, _targetDirectory);
            else
                Backup(_sourceFile, _targetDirectory);
        }

        private void Backup(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            var files = sourceDirectory.GetFiles();
            foreach (var fileObject in files)
                Backup(fileObject, targetDirectory);

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