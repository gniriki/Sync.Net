using System.IO;

namespace Sync.Net
{
    public interface IFileObject
    {
        string Name { get; }
        bool Exists { get; }
        Stream GetStream();
        void Create();
    }
}