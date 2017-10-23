using Amazon;
using Sync.Net.Configuration;

namespace Sync.Net
{
    public interface ISyncNetTaskFactory
    {
        ISyncNetTask Create(SyncNetConfiguration configuration);
    }
}