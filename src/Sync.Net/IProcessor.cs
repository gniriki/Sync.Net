using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(SyncNetProcessor sender, SyncNetProgressChangedEventArgs e);

    public interface IProcessor
    {
        Task ProcessSourceDirectoryAsync();
        event SyncNetProgressChangedDelegate ProgressChanged;
        Task ProcessFileAsync(IFileObject file);
        Task ProcessDirectoryAsync(IDirectoryObject directory);
    }
}