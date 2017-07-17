using System;
using System.IO;
using Sync.Net.IO;

namespace Sync.Net.TestHelpers
{
    public class MemoryFileObject : IFileObject
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly string _contents;

        public MemoryFileObject(string name)
        {
            Name = name;
        }

        public MemoryFileObject(string name, string contents) : this(name)
        {
            _contents = contents;
            ModifiedDate = DateTime.Now;

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(_contents);
                writer.Flush();
                _buffer = stream.ToArray();
            }
        }

        public MemoryFileObject(string name, string contents, DateTime modifiedDate) : this(name, contents)
        {
            ModifiedDate = modifiedDate;
        }

        public string Name { get; set; }
        public bool Exists => true;

        public long Size => _buffer.Length;
        public DateTime ModifiedDate { get; set; }

        public Stream GetStream()
        {
            return new MemoryStream(_buffer);
        }

        public void Create()
        {
        }
    }
}