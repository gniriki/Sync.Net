using Amazon;
using Sync.Net.Configuration;

namespace Sync.Net
{
    public interface IProcessorFactory
    {
        IProcessor Create(ProcessorConfiguration configuration);
    }
}