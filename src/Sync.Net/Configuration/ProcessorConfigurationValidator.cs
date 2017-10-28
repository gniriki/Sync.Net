using System;

namespace Sync.Net.Configuration
{
    public class ProcessorConfigurationValidator : IProcessorConfigurationValidator
    {
        public ProcessorConfigurationValidationResult Validate(ProcessorConfiguration configuration)
        {
            if (!configuration.CredentialsType.HasValue)
                return Invalid("Choose credentials");
            switch (configuration.CredentialsType.Value)
            {
                case Configuration.CredentialsType.DefaultProfile:
                    break;
                case Configuration.CredentialsType.NamedProfile:
                    if (String.IsNullOrEmpty(configuration.ProfileName))
                        return Invalid("Profile name cannot be empty");
                    break;
                case Configuration.CredentialsType.Basic:
                    if (String.IsNullOrEmpty(configuration.KeyId))
                        return Invalid("KeyId cannot be empty");
                    if (String.IsNullOrEmpty(configuration.KeySecret))
                        return Invalid("KeySecret cannot be empty");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Valid();
        }

        private ProcessorConfigurationValidationResult Valid()
        {
            return new ProcessorConfigurationValidationResult { IsValid = true };
        }

        private ProcessorConfigurationValidationResult Invalid(string message)
        {
            return new ProcessorConfigurationValidationResult { IsValid = false, Message = message };
        }
    }
}