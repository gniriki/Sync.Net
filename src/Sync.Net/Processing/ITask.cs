using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net.Processing
{
    public interface ITask
    {
        void Execute();
        Task ExecuteAsync();
        IFileObject File { get; }
    }
}