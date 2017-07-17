using System;
using System.IO;
using System.Linq;
using Sync.Net.IO;

namespace Sync.Net.Tests
{
    public class MemoryFileObject : IFileObject
    {
        private string _contents;
        private byte[] _buffer = new byte[1024];

        public MemoryFileObject(string name)
        {
            this.Name = name;
        }

        public MemoryFileObject(string name, string contents) : this(name)
        {
            this._contents = contents;
            this.ModifiedDate = DateTime.Now;
            
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(_contents);
                writer.Flush();
               _buffer = stream.ToArray();
            }
        }

        public MemoryFileObject(string name, string contents, DateTime modifiedDate) : this(name, contents)
        {
            this.ModifiedDate = modifiedDate;
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