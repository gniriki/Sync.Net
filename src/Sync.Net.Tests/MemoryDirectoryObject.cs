using System.Collections.Generic;

namespace Sync.Net.Tests
{
    public class MemoryDirectoryObject : IDirectoryObject
    {
        public Dictionary<string, MemoryFileObject> Files 
            = new Dictionary<string, MemoryFileObject>();

        public Dictionary<string, MemoryDirectoryObject> Directories 
            = new Dictionary<string, MemoryDirectoryObject>();

        public string Name { get; set; }

        public IEnumerable<IDirectoryObject> GetDirectories()
        {
            return Directories.Values;
        }

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

        public IEnumerable<IFileObject> GetFiles()
        {
            return Files.Values;
        }

        public MemoryDirectoryObject AddFile(MemoryFileObject memoryFileObject)
        {
            Files.Add(memoryFileObject.Name, memoryFileObject);
            return this;
        }

        public MemoryDirectoryObject AddFile(string fileName, string contents)
        {
            var memoryFileObject = new MemoryFileObject(fileName, contents);
            Files.Add(memoryFileObject.Name, memoryFileObject);
            return this;
        }

        public IDirectoryObject AddDirectory(MemoryDirectoryObject subDirectory)
        {
            Directories.Add(subDirectory.Name, subDirectory);
            return this;
        }

    }
}