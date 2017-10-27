using System.Collections.Generic;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public interface IDirectoryScanner
    {
        List<IFileObject> GetFilesToCopy();
    }
}