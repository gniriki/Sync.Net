using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetTests
    {
        [TestMethod]
        public void CreatesFileOnTargetFileSystem()
        {
            SyncNet syncNet = new SyncNet();
            var memoryDirectoryObject = new MemoryDirectoryObject();

            syncNet.Backup(new MemoryFileObject("file.txt"), memoryDirectoryObject);
            Assert.IsTrue(memoryDirectoryObject.ContainsFile("file.txt"));
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            SyncNet syncNet = new SyncNet();
            IDirectoryObject memoryDirectoryObject = new MemoryDirectoryObject();

            var fileName = "file.txt";
            var contents = "This is file content";

            syncNet.Backup(new MemoryFileObject(fileName, contents), memoryDirectoryObject);

            IFileObject targetFile = memoryDirectoryObject.GetFile(fileName);
            using (StreamReader sr = new StreamReader(targetFile.GetStream()))
            {
                string targetFileContents = sr.ReadToEnd().Replace("\0", string.Empty);
                Assert.AreEqual(contents, targetFileContents);
            }
        }
    }

    public class MemoryDirectoryObject : IDirectoryObject
    {
        public Dictionary<string, MemoryFileObject> Files = new Dictionary<string, MemoryFileObject>();

        public bool ContainsFile(string name)
        {
            return Files.ContainsKey(name);
        }

        public void CreateFile(string name)
        {
            Files.Add(name, new MemoryFileObject(name));
        }

        public IFileObject GetFile(string name)
        {
            return Files[name];
        }
    }
    
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
        public Stream GetStream()
        {
            MemoryStream stream = new MemoryStream(_buffer);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(_contents);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
