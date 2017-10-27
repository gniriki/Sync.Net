using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(Processor sender, SyncNetProgressChangedEventArgs e);

    public interface IProcessor
    {
        Task ProcessSourceDirectoryAsync();
        event SyncNetProgressChangedDelegate ProgressChanged;
        Task CopyFileAsync(IFileObject file);
        Task ProcessDirectoryAsync(IDirectoryObject directory);
    }
}