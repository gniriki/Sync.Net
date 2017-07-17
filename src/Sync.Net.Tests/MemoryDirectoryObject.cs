using System.Collections.Generic;

namespace Sync.Net.Tests
{
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
    }
}