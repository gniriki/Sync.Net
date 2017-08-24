using System;

namespace Sync.Net.UI.Utils
{
    public class Logger : ILogger
    {
        public event LogUpdatedEventHandler LogUpdated;
        public string Contents { get; set; }

        public void Log(string line)
        {
            var newLine = $"{DateTime.Now.ToLongTimeString()} {line}\n";
            Contents += newLine;
            OnLogUpdated(newLine);
        }

        protected virtual void OnLogUpdated(string newline)
        {
            LogUpdated?.Invoke(newline);
        }
    }
}