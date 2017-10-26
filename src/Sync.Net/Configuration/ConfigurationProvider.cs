using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sync.Net.Configuration
{
    public interface IConfigurationProvider
    {
        SyncNetConfiguration Current { get; }
        void Save();
        void Create();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private static SyncNetConfiguration _configuration;
        private IConfigFile _configFile;
        private static readonly DataContractSerializer Serializer;

        static ConfigurationProvider()
        {
            Serializer = new DataContractSerializer(typeof(SyncNetConfiguration));
        }

        public ConfigurationProvider(IConfigFile configFile)
        {
            _configFile = configFile;
        }

        public SyncNetConfiguration Current
        {
            get
            {
                if(_configuration == null)
                    _configuration = Load();
                return _configuration;
            }
        }

        public void Save()
        {
            var stream = _configFile.GetStream();
            using (stream)
            {
                _configuration.Validate();
                Serializer.WriteObject(stream, _configuration);
            }
        }

        private SyncNetConfiguration Load()
        {
            if (!_configFile.Exists())
                throw new ConfigurationLoadException("No configuration file found!");

            var stream = _configFile.GetStream();
            using (stream)
            {
                if (stream.Length == 0)
                    throw new ConfigurationLoadException("The configuration file is empty");

                return Serializer.ReadObject(stream) as SyncNetConfiguration;
            }
        }

        public void Create()
        {
            _configuration = new SyncNetConfiguration();
        }
    }
}
