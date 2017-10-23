using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.Configuration
{
    public class ConfigurationLoadException : Exception
    {
        public ConfigurationLoadException()
        {
        }

        public ConfigurationLoadException(string message) : base(message)
        {
        }

        public ConfigurationLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
