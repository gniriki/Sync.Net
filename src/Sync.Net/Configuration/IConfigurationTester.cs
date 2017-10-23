namespace Sync.Net.Configuration
{
    public interface IConfigurationTester
    {
        ConfigurationTestResult Test(SyncNetConfiguration configuration);
    }
}