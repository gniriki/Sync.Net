using System.IO;

namespace Sync.Net
{
    public interface IFileObject
    {
        string Name { get; }
        bool Exists { get; }
        long Size { get; }
        Stream GetStream();
        void Create();
    }
}