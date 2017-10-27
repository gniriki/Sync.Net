using Amazon;
using Sync.Net.Configuration;
using Sync.Net.Processing;

namespace Sync.Net
{
    public interface IProcessorFactory
    {
        IProcessor Create(ProcessorConfiguration configuration, AsyncTaskQueue taskQueue);
    }
}