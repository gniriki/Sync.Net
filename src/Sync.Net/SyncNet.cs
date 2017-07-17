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

            IFileObject targetFile = targetDirectory.GetFile(file.Name);

            using (var stream = file.GetStream())
            {
                using (var destination = targetFile.GetStream())
                {
                    stream.CopyTo(destination);
                }
            }
        }
    }
}
