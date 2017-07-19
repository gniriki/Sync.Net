using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.UI.Utils
{
    public class ConfigFile : IConfigFile
    {
        private string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".SyncNet", "SyncNet.config");

        public Stream GetStream()
        {
            FileInfo fi = new FileInfo(_path);
            if(!fi.Directory.Exists)
                fi.Directory.Create();
            return fi.Open(FileMode.OpenOrCreate);
        }
    }
}
