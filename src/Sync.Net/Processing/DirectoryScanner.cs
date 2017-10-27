using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public class DirectoryScanner
    {
        private IDirectoryObject _source;
        private IDirectoryObject _target;

        public DirectoryScanner(IDirectoryObject source, IDirectoryObject target)
        {
            _source = source;
            _target = target;
        }

        public List<IFileObject> GetFilesToCopy()
        {
            return GetFilesToCopy(_source, _target);
        }

        private List<IFileObject> GetFilesToCopy(IDirectoryObject source, IDirectoryObject target)
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
                filesToUpload.AddRange(GetFilesToCopy(sourceSubDirectory, targetSubDirectory));
            }

            return filesToUpload;
        }
    }
}
