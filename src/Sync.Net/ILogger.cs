namespace Sync.Net
{
    public delegate void LogUpdatedEventHandler(string newLine);

    public interface ILogger
    {
        string Contents { get; set; }
        void Log(string line);
        event LogUpdatedEventHandler LogUpdated;
    }
}