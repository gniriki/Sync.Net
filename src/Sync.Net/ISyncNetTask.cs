namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e);

    public interface ISyncNetTask
    {
        void ProcessFiles();
        event SyncNetProgressChangedDelegate ProgressChanged;
        void ProcessFile(string filePath);
    }
}