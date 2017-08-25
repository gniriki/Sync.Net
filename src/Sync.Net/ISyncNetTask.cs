using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e);

    public interface ISyncNetTask
    {
        Task ProcessFilesAsync();
        event SyncNetProgressChangedDelegate ProgressChanged;
        Task ProcessFileAsync(IFileObject file);
        Task ProcessDirectoryAsync(string path);
    }
}