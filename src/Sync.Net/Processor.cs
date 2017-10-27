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

        private readonly IDirectoryObject _sourceDirectory;
        private readonly IDirectoryObject _targetDirectory;
        private long _processedBytes;
        private int _processedFiles;
        private long _totalBytes;
        private int _totalFiles;

        private ITaskQueue _queue;

        public Processor(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory, ITaskQueue queue)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _queue = queue;
        }

        public event SyncNetProgressChangedDelegate ProgressChanged;

        public void ProcessSourceDirectory()
        {
            StaticLogger.Log("Scaning directory for changes.");
            var scanner = new DirectoryScanner(_sourceDirectory, _targetDirectory);
            var files = scanner.GetFilesToCopy();
            StaticLogger.Log($"Found {files.Count} new files.");
            foreach (var file in files)
            {
                CopyFile(file);
            }
        }

        public void ProcessDirectory(IDirectoryObject directory)
        {
            var targetDirectory = GetTargetDirectory(directory.FullName);

            var scanner = new DirectoryScanner(directory, targetDirectory);
            var files = scanner.GetFilesToCopy();
            foreach (var file in files)
            {
                CopyFile(file);
            }
        }

        public void CopyFile(IFileObject file)
        {
            var targetDirectory = GetTargetDirectory(file.FullName);
            var copyFileTask = new CopyFileTask(file, targetDirectory);
            _queue.Queue(copyFileTask);
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

        private void FileProcessingCompleted(IFileObject currentFile)
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