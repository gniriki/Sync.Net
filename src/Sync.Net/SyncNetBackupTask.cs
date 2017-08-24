using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime.Internal;
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
        private readonly IDirectoryObject _targetDirectory;
        private readonly List<IFileObject> _filesToBackup = new List<IFileObject>();
        private long _processedBytes;
        private int _processedFiles;
        private long _totalBytes;
        private int _totalFiles;

        public SyncNetBackupTask(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public void Run()
        {
            StaticLogger.Log("Preparing...");
            var files = GetFilesToUpload(_sourceDirectory, _targetDirectory);
            _totalFiles = files.Count();
            foreach (var fileObject in files)
                _totalBytes += fileObject.Size;

            StaticLogger.Log("Uploading...");
            foreach (var fileObject in files)
            {
                ProcessFile(fileObject.FullName);
            }

            StaticLogger.Log("Done.");
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        public void ProcessFile(string fileName)
        {
            if (isAbsolute(fileName))
            {
                fileName = fileName.Replace(_sourceDirectory.FullName, string.Empty);
            }

            var sourceDirectory = _sourceDirectory;
            var targetDirectory = _targetDirectory;
            var file = fileName;

            if (file.Contains('\\'))
            {
                if (file.StartsWith(".\\"))
                    file = file.Substring(2);

                var parts = file.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < parts.Length - 1; i++)
                {
                    sourceDirectory = sourceDirectory.GetDirectory(parts[i]);
                    targetDirectory = targetDirectory.GetDirectory(parts[i]);
                }

                file = parts[parts.Length - 1];
            }

            Backup(sourceDirectory.GetFile(file), targetDirectory);
        }

        private bool isAbsolute(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && !fileName.StartsWith(".");
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
            if (!targetDirectory.Exists)
                targetDirectory.Create();

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

        protected virtual void OnProgressChanged(SyncNetProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}