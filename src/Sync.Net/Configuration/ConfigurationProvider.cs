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
        ProcessorConfiguration Current { get; }
        void Save();
        void Create();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private static ProcessorConfiguration _configuration;
        private IConfigFile _configFile;
        private static readonly DataContractSerializer Serializer;
        private IProcessorConfigurationValidator _validator;

        static ConfigurationProvider()
        {
            Serializer = new DataContractSerializer(typeof(ProcessorConfiguration));
        }

        public ConfigurationProvider(IConfigFile configFile, IProcessorConfigurationValidator validator)
        {
            _configFile = configFile;
            _validator = validator;
        }

        public ProcessorConfiguration Current
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
                var result = _validator.Validate(_configuration);
                if(!result.IsValid)
                    throw new ConfigurationException(result.Message);
                Serializer.WriteObject(stream, _configuration);
            }
        }

        public ProcessorConfiguration Load()
        {
            if (!_configFile.Exists())
                throw new ConfigurationLoadException("No configuration file found!");

            var stream = _configFile.GetStream();
            using (stream)
            {
                if (stream.Length == 0)
                    throw new ConfigurationLoadException("The configuration file is empty");

                return Serializer.ReadObject(stream) as ProcessorConfiguration;
            }
        }

        public void Create()
        {
            _configuration = new ProcessorConfiguration();
        }
    }
}
