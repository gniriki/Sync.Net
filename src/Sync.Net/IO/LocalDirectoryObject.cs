using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.IO
{
    public class LocalDirectoryObject : IDirectoryObject
    {
        private DirectoryInfo _directoryInfo;

        public LocalDirectoryObject(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        public LocalDirectoryObject(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public string Name => _directoryInfo.Name;
        public bool Exists => _directoryInfo.Exists;

        public IFileObject GetFile(string name)
        {
            return new LocalFileObject(Path.Combine(_directoryInfo.FullName, name));
        }

        public IEnumerable<IFileObject> GetFiles(bool recursive = false)
        {
            return _directoryInfo.GetFiles().Select(x => new LocalFileObject(x));
        }

        public IEnumerable<IDirectoryObject> GetDirectories()
        {
            return _directoryInfo.GetDirectories().Select(x => new LocalDirectoryObject(x));
        }

        public void Create()
        {
            _directoryInfo.Create();
        }

        public IDirectoryObject GetDirectory(string name)
        {
            return new LocalDirectoryObject(Path.Combine(_directoryInfo.FullName, name));
        }
    }
}
