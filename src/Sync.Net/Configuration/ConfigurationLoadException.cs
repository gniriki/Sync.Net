using System;

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
