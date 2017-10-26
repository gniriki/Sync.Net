namespace Sync.Net.Configuration
{
    public interface IConfigurationTester
    {
        ConfigurationTestResult Test(ProcessorConfiguration configuration);
    }
}