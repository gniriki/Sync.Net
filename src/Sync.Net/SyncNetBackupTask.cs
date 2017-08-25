using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime.Internal;
using Sync.Net.IO;
using System.Threading.Tasks;
using Nito.AsyncEx;

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

        public async Task ProcessFilesAsync()
        {
            StaticLogger.Log("Preparing...");
            var files = GetFilesToUpload(_sourceDirectory, _targetDirectory);

            foreach (var fileObject in files)
            {
                UpdateProgressQueue(fileObject);
            }

            StaticLogger.Log("Uploading...");
            foreach (var fileObject in files)
            {
                await ProcessFileAsync(fileObject);
            }

            StaticLogger.Log("Done.");
        }

        private void UpdateProgressQueue(IFileObject fileObject)
        {
            if (_filesToBackup.All(x => x.FullName != fileObject.FullName))
                _filesToBackup.Add(fileObject);

            _totalFiles = _filesToBackup.Count;
            _totalBytes = _filesToBackup.Sum(file => file.Size);
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        private readonly AsyncLock _mutex = new AsyncLock();

        public async Task ProcessFileAsync(IFileObject file)
        {
            using (await _mutex.LockAsync())
            {
                await Task.Run(() => ProcessFile(file));
            }
        }

        public Task ProcessDirectoryAsync(IDirectoryObject directory)
        {
            throw new NotImplementedException();
        }

        private void ProcessFile(IFileObject file)
        {
            var targetDirectory = GetTargetDirectory(file);

            UpdateProgressQueue(file);
            Backup(file, targetDirectory);
        }

        private IDirectoryObject GetTargetDirectory(IFileObject file)
        {
            var filePath = file.FullName;

            if (isAbsolute(filePath))
            {
                filePath = filePath.Replace(_sourceDirectory.FullName, string.Empty);
            }

            var sourceDirectory = _sourceDirectory;
            var targetDirectory = _targetDirectory;

            if (filePath.Contains('\\'))
            {
                if (filePath.StartsWith(".\\"))
                    filePath = filePath.Substring(2);

                var parts = filePath.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < parts.Length - 1; i++)
                {
                    sourceDirectory = sourceDirectory.GetDirectory(parts[i]);
                    targetDirectory = targetDirectory.GetDirectory(parts[i]);
                }
            }

            return targetDirectory;
        }

        private bool isAbsolute(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && !fileName.StartsWith(".");
        }

        private static IEnumerable<IFileObject> GetFilesToUpload(IDirectoryObject source, IDirectoryObject target)
        {
            var filesToUpload = new List<IFileObject>();
            var sourceFiles = source.GetFiles();

            //filesToUpload.AddRange(sourceFiles);
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

                using (var stream = file.GetStream())
                {
                    using (var destination = targetFile.GetStream())
                    {
                        stream.CopyTo(destination);
                    }
                }

                targetFile.ModifiedDate = file.ModifiedDate;
                UpdateProgess(file);
            }
            else
            {
                UpdateProgess(file);
            }
        }

        private void UpdateProgess(IFileObject currentFile)
        {
            _processedFiles++;
            _processedBytes += currentFile.Size;
            OnProgressChanged(
                new SyncNetProgressChangedEventArgs
                {
                    ProcessedFiles = _processedFiles,
                    TotalFiles = _totalFiles,
                    ProcessedBytes = _processedBytes,
                    TotalBytes = _totalBytes,
                    CurrentFile = currentFile
                });

            StaticLogger.Log($"Processing file {_processedFiles}/{_totalFiles}. Current file: {currentFile.FullName}");
        }

        protected virtual void OnProgressChanged(SyncNetProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}