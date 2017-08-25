using System.Threading.Tasks;

namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e);

    public interface ISyncNetTask
    {
        Task ProcessFilesAsync();
        event SyncNetProgressChangedDelegate ProgressChanged;
        Task ProcessFileAsync(string path);
        Task ProcessDirectoryAsync(string path);
    }
}