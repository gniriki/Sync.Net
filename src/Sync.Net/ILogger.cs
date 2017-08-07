using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net
{
    public delegate void LogUpdatedEventHandler(string newLine);

    public interface ILogger
    {
        void Log(string line);
        event LogUpdatedEventHandler LogUpdated;
        string Contents { get; set; }
    }
}
