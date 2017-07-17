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
    }

    public class MemoryDirectoryObject : IDirectoryObject
    {
        public Dictionary<string, MemoryFileObject> Files = new Dictionary<string, MemoryFileObject>();

        public bool ContainsFile(string fileName)
        {
            return Files.ContainsKey(fileName);
        }

        public void CreateFile(string fileName)
        {
            Files.Add(fileName, new MemoryFileObject(fileName));
        }
    }

    public class MemoryFileObject : IFileObject
    {
        public MemoryFileObject(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
