using System;
using System.IO;

namespace Sync.Net.UI.Utils
{
    public class ConfigFile : IConfigFile
    {
        private readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".SyncNet", "SyncNet.config");

        public Stream GetStream()
        {
            var fi = new FileInfo(_path);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            return fi.Open(FileMode.OpenOrCreate);
        }

        public void Clear()
        {
            File.WriteAllText(_path, String.Empty);
        }
    }
}