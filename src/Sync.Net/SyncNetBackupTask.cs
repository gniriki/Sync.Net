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


    public class SyncNetBackupTask : ISyncNetTask
    {
        private readonly IDirectoryObject _sourceDirectory;
        private readonly IFileObject _sourceFile;
        private readonly IDirectoryObject _targetDirectory;
        private long _processedBytes;
        private int _processedFiles;
        private long _totalBytes;
        private int _totalFiles;

        public SyncNetBackupTask(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public SyncNetBackupTask(IFileObject sourceFile, IDirectoryObject targetDirectory)
        {
            _sourceFile = sourceFile;
            _targetDirectory = targetDirectory;
        }

        public void Run()
        {
            if (_sourceDirectory != null)
            {
                StaticLogger.Log("Preparing...");
                var files = GetFilesToUpload(_sourceDirectory, _targetDirectory);
                _totalFiles = files.Count();
                foreach (var fileObject in files)
                    _totalBytes += fileObject.Size;

                StaticLogger.Log("Uploading...");
                Backup(_sourceDirectory, _targetDirectory);

                StaticLogger.Log("Done.");
            }
            else
            {
                _totalBytes += _sourceFile.Size;
                Backup(_sourceFile, _targetDirectory);
            }
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        public void UpdateFile(string fileName)
        {
            var sourceDirectory = _sourceDirectory;
            var targetDirectory = _targetDirectory;
            var file = fileName;

            if (file.Contains('\\'))
            {
                if (file.StartsWith(".\\"))
                    file = file.Substring(2);

                var parts = file.Split('\\');

                for (var i = 0; i < parts.Length - 1; i++)
                {
                    sourceDirectory = sourceDirectory.GetDirectory(parts[i]);
                    targetDirectory = targetDirectory.GetDirectory(parts[i]);
                }

                file = parts[parts.Length - 1];
            }

            Backup(sourceDirectory.GetFile(file), targetDirectory);
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

        protected virtual void OnProgressChanged(SyncNetProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}