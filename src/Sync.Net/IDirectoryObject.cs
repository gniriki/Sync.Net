namespace Sync.Net
{
    public interface IDirectoryObject
    {
        bool ContainsFile(string fileName);
        void CreateFile(string fileName);
    }
}