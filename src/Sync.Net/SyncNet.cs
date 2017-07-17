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
    }

    public delegate void SyncNetProgressChangedDelegate(SyncNet sender, SyncNetProgressChangedEventArgs e);

    public class SyncNet
    {
        private IDirectoryObject _sourceDirectory;
        private IDirectoryObject _targetDirectory;
        private IFileObject _sourceFile;

        public SyncNet(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
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

            using (var stream = file.GetStream())
            {
                using (var destination = targetFile.GetStream())
                {
                    stream.CopyTo(destination);
                }
            }
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
    }
}
