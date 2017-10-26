using System.IO;

namespace Sync.Net.Configuration
{
    public interface IConfigFile
    {
        Stream GetStream();
        void Clear();
        bool Exists();
    }
}