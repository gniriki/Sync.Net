using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Sync.Net.IO;
using Sync.Net.Processing;

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


    public class Processor : IProcessor
    {
        private readonly List<IFileObject> _filesToBackup = new List<IFileObject>();

        private readonly AsyncLock _mutex = new AsyncLock();
        private readonly IDirectoryObject _sourceDirectory;
        private readonly IDirectoryObject _targetDirectory;
        private long _processedBytes;
        private int _processedFiles;
        private long _totalBytes;
        private int _totalFiles;

        public Processor(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public async Task ProcessSourceDirectoryAsync()
        {
            StaticLogger.Log("Preparing...");

            await ProcessDirectoryAsync(_sourceDirectory, _targetDirectory);

            StaticLogger.Log("Done.");
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        public async Task CopyFileAsync(IFileObject file)
        {
            using (await _mutex.LockAsync())
            {
                var targetDirectory = GetTargetDirectory(file.FullName);

                var copyFileTask = new CopyFileTask(file, targetDirectory);
                UpdateProgressQueue(file);
                await copyFileTask.ExecuteAsync();
                UpdateProgess(file);
            }
        }

        public async Task ProcessDirectoryAsync(IDirectoryObject directory)
        {
            var targetDirectory = GetTargetDirectory(directory.FullName);
            await ProcessDirectoryAsync(directory, targetDirectory);
        }

        private async Task ProcessDirectoryAsync(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            StaticLogger.Log($"Processing directory {sourceDirectory.FullName}");
            var files = GetFilesToUpload(sourceDirectory, targetDirectory);
            var tasks = new List<ITask>();

            foreach (var fileObject in files)
            {
                UpdateProgressQueue(fileObject);

//                var targetDir = GetTargetDirectory(fileObject.FullName);
//                var copyFileTask = new CopyFileTask(fileObject, targetDir);
//                tasks.Add(copyFileTask);
            }

            foreach (var fileObject in files)
                await CopyFileAsync(fileObject);
        }

        private void UpdateProgressQueue(IFileObject fileObject)
        {
            if (_filesToBackup.All(x => x.FullName != fileObject.FullName))
                _filesToBackup.Add(fileObject);

            _totalFiles = _filesToBackup.Count;
            _totalBytes = _filesToBackup.Sum(file => file.Size);
        }

        private IDirectoryObject GetTargetDirectory(string path)
        {
            if (isAbsolute(path))
                path = path.Replace(_sourceDirectory.FullName, string.Empty);

            var targetDirectory = _targetDirectory;

            if (path.Contains('\\'))
            {
                if (path.StartsWith(".\\"))
                    path = path.Substring(2);

                var parts = path.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < parts.Length - 1; i++)
                    targetDirectory = targetDirectory.GetDirectory(parts[i]);
            }

            return targetDirectory;
        }

        private bool isAbsolute(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && !fileName.StartsWith(".");
        }

        private static List<IFileObject> GetFilesToUpload(IDirectoryObject source, IDirectoryObject target)
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