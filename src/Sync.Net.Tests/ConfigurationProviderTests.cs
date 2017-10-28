using System.IO;
using Amazon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ConfigurationProviderTests
    {
        private readonly string _configLocalDirectory = "testLocal";
        private readonly string _configS3Bucket = "testBucket";
        private Mock<IConfigFile> _configFile;
        private Mock<IProcessorConfigurationValidator> _validator;
        private ConfigurationProvider _configurationProvider;

        [TestInitialize]
        public void Init()
        {
            _configFile = new Mock<IConfigFile>();
            _validator = new Mock<IProcessorConfigurationValidator>();
            _configFile.Setup(x => x.GetStream()).Returns(new MemoryStream());
            _validator.Setup(x => x.Validate(It.IsAny<ProcessorConfiguration>()))
                .Returns(new ProcessorConfigurationValidationResult
                {
                    IsValid = false
                });
            _configurationProvider = new ConfigurationProvider(_configFile.Object, _validator.Object);

        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationException))]
        public void SaveThrowsErrorWhenKeyIdIsEmpty()
        {
            _configurationProvider.Create();
            _configurationProvider.Current.CredentialsType = CredentialsType.Basic;
            _configurationProvider.Current.KeySecret = "secret";

            _configurationProvider.Save();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationException))]
        public void SaveThrowsErrorWhenKeySecretIsEmpty()
        {
            _configurationProvider.Create();
            _configurationProvider.Current.CredentialsType = CredentialsType.Basic;
            _configurationProvider.Current.KeyId = "id";

            _configurationProvider.Save();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationException))]
        public void SaveThrowsErrorWhenProfileNameIsEmpty()
        {
            _configurationProvider.Create();
            _configurationProvider.Current.CredentialsType = CredentialsType.NamedProfile;
            _configurationProvider.Save();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationLoadException))]
        public void LoadThrowsErrorWhenStreamIsEmpty()
        {
            var configuration = _configurationProvider.Load();
        }
    }
}