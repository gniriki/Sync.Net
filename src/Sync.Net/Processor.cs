﻿using System;
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
        public int FilesLeft;
    }


    public class Processor : IProcessor
    {
        private readonly IDirectoryObject _sourceDirectory;
        private readonly IDirectoryObject _targetDirectory;

        private ITaskQueue _queue;

        public Processor(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory, ITaskQueue queue)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _queue = queue;
            _queue.TaskStarting += _queue_TaskStarting;
            _queue.TaskCompleted += _queue_TaskCompleted;
        }

        private void _queue_TaskStarting(TaskQueueEventArgs eventArgs)
        {
            StaticLogger.Log("Starting task: " + eventArgs.Task);
        }

        private void _queue_TaskCompleted(TaskQueueEventArgs eventArgs)
        {
            StaticLogger.Log("Completed task: " + eventArgs.Task);
            FileProcessingCompleted(eventArgs.Task.File);
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
            QueueTask(copyFileTask);
        }

        private void QueueTask(ITask task)
        {
            StaticLogger.Log($"Adding task to queue: {task}");
            _queue.Queue(task);
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
            OnProgressChanged(
                new SyncNetProgressChangedEventArgs
                {
                    FilesLeft = _queue.Count,
                });

            StaticLogger.Log($"{_queue.Count} files left to process.");
        }

        protected virtual void OnProgressChanged(SyncNetProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        public void RenameFile(IFileObject fileToRename, string newName)
        {
            var fileObject = GetTargetFile(fileToRename);
            QueueTask(new RenameFileTask(fileObject, newName));
        }

        private IFileObject GetTargetFile(IFileObject file)
        {
            var directory = GetTargetDirectory(file.FullName);
            var fileObject = directory.GetFile(file.Name);
            return fileObject;
        }
    }
}