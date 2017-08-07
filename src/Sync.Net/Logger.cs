using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net
{
    public static class StaticLogger
    {
        public static ILogger Logger;

        public static void Log(string newLine)
        {
            Logger?.Log(newLine);
        }
    }
}
