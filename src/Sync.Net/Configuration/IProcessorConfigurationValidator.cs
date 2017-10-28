namespace Sync.Net.Configuration
{
    public interface IProcessorConfigurationValidator
    {
        ProcessorConfigurationValidationResult Validate(ProcessorConfiguration configuration);
    }
}