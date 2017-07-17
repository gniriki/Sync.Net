using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net
{
    public class SyncNet
    {
        public void Backup(IFileObject file, IDirectoryObject targetDirectory)
        {
            if (!targetDirectory.ContainsFile(file.Name))
            {
                targetDirectory.CreateFile(file.Name);
            }
        }
    }
}
