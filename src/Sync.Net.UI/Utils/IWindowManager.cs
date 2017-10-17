namespace Sync.Net.UI.Utils
{
    public interface IWindowManager
    {
        string ShowDirectoryDialog();
        void ShutdownApplication();
        void ShowMessage(string message);
    }
}