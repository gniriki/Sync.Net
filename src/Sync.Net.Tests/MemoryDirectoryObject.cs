using System;
using System.Collections.Generic;
using System.Linq;
using Sync.Net.IO;

namespace Sync.Net.Tests
{
    public class MemoryDirectoryObject : IDirectoryObject
    {
        public Dictionary<string, MemoryDirectoryObject> Directories
            = new Dictionary<string, MemoryDirectoryObject>();

        public Dictionary<string, MemoryFileObject> Files
            = new Dictionary<string, MemoryFileObject>();

        public MemoryDirectoryObject(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Exists => true;

        public IEnumerable<IDirectoryObject> GetDirectories()
        {
            return Directories.Values;
        }

        public void Create()
        {
        }

        public IDirectoryObject GetDirectory(string name)
        {
            MemoryDirectoryObject directory = null;
            if (!Directories.ContainsKey(name))
            {
                directory = new MemoryDirectoryObject(name);
                AddDirectory(directory);
            }
            else
            {
                directory = Directories[name];
            }

            return directory;
        }

        public IFileObject GetFile(string name)
        {
            MemoryFileObject file = null;
            if (!Files.ContainsKey(name))
            {
                file = new MemoryFileObject(name);
                AddFile(file);
            }
            else
            {
                file = Files[name];
            }

            return file;
        }

        public IEnumerable<IFileObject> GetFiles(bool recursive = false)
        {
            var list = Files.Values.Cast<IFileObject>().ToList();
            if (recursive)
                foreach (var directoryObject in GetDirectories())
                    list.AddRange(directoryObject.GetFiles(true));

            return list;
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

        public MemoryDirectoryObject AddFile(string fileName, string contents, DateTime modifiedDate)
        {
            var memoryFileObject = new MemoryFileObject(fileName, contents, modifiedDate);
            Files.Add(memoryFileObject.Name, memoryFileObject);
            return this;
        }

        public IDirectoryObject AddDirectory(MemoryDirectoryObject subDirectory)
        {
            Directories.Add(subDirectory.Name, subDirectory);
            return this;
        }

        public bool ContainsFile(string name)
        {
            return Files.ContainsKey(name);
        }
    }
}