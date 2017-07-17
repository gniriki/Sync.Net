using System.IO;

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

            using (MemoryStream stream = new MemoryStream(_buffer))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(_contents);
                writer.Flush();
            }
        }

        public string Name { get; set; }
        public bool Exists => true;

        public Stream GetStream()
        {
            MemoryStream stream = new MemoryStream(_buffer);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(_contents);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void Create()
        {
            
        }
    }
}