namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e);

    public interface ISyncNetTask
    {
        void Run();
        event SyncNetProgressChangedDelegate ProgressChanged;
        void UpdateFile(string fileName);
    }
}