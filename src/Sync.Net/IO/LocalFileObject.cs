using System;
using System.IO;

namespace Sync.Net.IO
{
    public class LocalFileObject : IFileObject
    {
        private readonly FileInfo _fileInfo;

        public LocalFileObject(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        public LocalFileObject(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
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
            using (var stream = _fileInfo.Create())
            {
                ;
            }
        }

        public void Rename(string newName)
        {
            throw new NotImplementedException();
        }

        public string FullName => _fileInfo.FullName;

        public bool IsReady
        {
            get
            {
                try
                {
                    using (FileStream inputStream =
                        File.Open(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (inputStream.Length > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}