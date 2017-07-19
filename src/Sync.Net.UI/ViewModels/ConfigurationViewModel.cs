using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Microsoft.Win32;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private SyncNetConfiguration _configuration;
        private IWindowManager _windowManager;
        private IConfigFile _configFile;

        public ConfigurationViewModel(SyncNetConfiguration configuration, IWindowManager windowManager, IConfigFile configFile)
        {
            _configuration = configuration;
            _windowManager = windowManager;
            _configFile = configFile;
            SelectFile = new RelayCommand(
                p => true,
                p =>
                {
                    LocalDirectory = _windowManager.ShowDirectoryDialog();
                });
        }

        public string LocalDirectory
        {
            get { return _configuration.LocalDirectory; }
            set
            {
                _configuration.LocalDirectory = value;
                OnPropertyChanged();
            }
        }

        public string S3Bucket
        {
            get { return _configuration.S3Bucket; }
            set { _configuration.S3Bucket = value;
                OnPropertyChanged();
            }
        }

        public RegionEndpoint RegionEndpoint
        {
            get { return _configuration.RegionEndpoint; }
            set
            {
                _configuration.RegionEndpoint = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SelectFile
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            using(var stream = _configFile.GetStream())
                _configuration.Save(stream);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
