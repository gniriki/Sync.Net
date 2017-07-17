using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.IO
{
    public class LocalFileObject : IFileObject
    {
        private FileInfo _fileInfo;

        public LocalFileObject(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        public LocalFileObject(FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
        }

        public string Name => _fileInfo.Name;

        public bool Exists => _fileInfo.Exists;

        public long Size => _fileInfo.Length;

        public DateTime ModifiedDate
        {
            get { return _fileInfo.LastWriteTime; }
            set { _fileInfo.LastWriteTime = value; }
        }

        public Stream GetStream()
        {
            return _fileInfo.Open(FileMode.OpenOrCreate);
        }

        public void Create()
        {
            using (var stream = _fileInfo.Create());
        }
    }
}
