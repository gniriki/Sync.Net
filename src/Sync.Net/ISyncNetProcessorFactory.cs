using Amazon;
using Sync.Net.Configuration;

namespace Sync.Net
{
    public interface ISyncNetProcessorFactory
    {
        IProcessor Create(SyncNetConfiguration configuration);
    }
}