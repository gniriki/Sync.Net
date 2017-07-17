using System.Collections.Generic;

namespace Sync.Net
{
    public interface IDirectoryObject
    {
        bool ContainsFile(string name);
        void CreateFile(string name);
        IFileObject GetFile(string fileName);
        IEnumerable<IFileObject> GetFiles();
        string Name { get; }
        bool Exists { get; }
        IEnumerable<IDirectoryObject> GetDirectories();
        void Create();
        IDirectoryObject GetDirectory(string name);
    }
}