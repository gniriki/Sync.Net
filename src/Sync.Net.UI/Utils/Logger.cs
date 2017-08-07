namespace Sync.Net.UI.Utils
{
    public class Logger : ILogger
    {
        public event LogUpdatedEventHandler LogUpdated;
        public string Contents { get; set; }

        public void Log(string line)
        {
            Contents += line + "\n";
            OnLogUpdated(line + "\n");
        }

        protected virtual void OnLogUpdated(string newline)
        {
            LogUpdated?.Invoke(newline);
        }
    }
}