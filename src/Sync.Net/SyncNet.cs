using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net
{
    public class SyncNet
    {
        public void Backup(IFileObject file, IDirectoryObject targetDirectory)
        {
            if (!targetDirectory.ContainsFile(file.Name))
            {
                targetDirectory.CreateFile(file.Name);
            }

            IFileObject targetFile = targetDirectory.GetFile(file.Name);

            using (var stream = file.GetStream())
            {
                using (var destination = targetFile.GetStream())
                {
                    stream.CopyTo(destination);
                }
            }
        }

        public void Backup(IDirectoryObject sourceDirectory, IDirectoryObject targetDirectory)
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
    }
}
