using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Sync.Net.Configuration;

namespace Sync.Net
{
    public class AmazonS3ClientFactory
    {
        public AmazonS3Client GetS3Client(SyncNetConfiguration configuration)
        {
            switch (configuration.CredentialsType)
            {
                case CredentialsType.NamedProfile:
                    var file = new SharedCredentialsFile();
                    CredentialProfile profile;
                    if (file.TryGetProfile(configuration.ProfileName, out profile))
                    {
                        return new AmazonS3Client(AWSCredentialsFactory.GetAWSCredentials(profile, null), configuration.RegionEndpoint);
                    }
                    else
                        goto default;
                case CredentialsType.Basic:
                    var basicCredentials = new BasicAWSCredentials(configuration.KeyId, configuration.KeySecret);
                    return new AmazonS3Client(basicCredentials, configuration.RegionEndpoint);
                default:
                    return new AmazonS3Client(configuration.RegionEndpoint);
            }
        }
    }
}