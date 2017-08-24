using System.Collections.Generic;

namespace Sync.Net.IO
{
    public interface IDirectoryObject
    {
        string Name { get; }
        bool Exists { get; }
        string FullName { get; }
        IFileObject GetFile(string name);
        IEnumerable<IFileObject> GetFiles(bool recursive = false);
        IEnumerable<IDirectoryObject> GetDirectories();
        void Create();
        IDirectoryObject GetDirectory(string name);
    }
}