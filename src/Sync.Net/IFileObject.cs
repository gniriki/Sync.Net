using System.IO;

namespace Sync.Net
{
    public interface IFileObject
    {
        string Name { get; set; }
        Stream GetStream();
    }
}