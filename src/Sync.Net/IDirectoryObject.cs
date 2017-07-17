namespace Sync.Net
{
    public interface IDirectoryObject
    {
        bool ContainsFile(string name);
        void CreateFile(string name);
        IFileObject GetFile(string name);
    }
}