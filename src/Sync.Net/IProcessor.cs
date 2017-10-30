using System.Threading.Tasks;
using Sync.Net.IO;

namespace Sync.Net
{
    public delegate void SyncNetProgressChangedDelegate(Processor sender, SyncNetProgressChangedEventArgs e);

    public interface IProcessor
    {
        event SyncNetProgressChangedDelegate ProgressChanged;
        void ProcessSourceDirectory();
        void CopyFile(IFileObject file);
        void ProcessDirectory(IDirectoryObject directory);
        void RenameFile(IFileObject fileToRename, string newName);
        void RenameDirectory(IDirectoryObject directoryToRename, string newName);
    }
}