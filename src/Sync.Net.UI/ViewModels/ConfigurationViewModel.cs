﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Amazon;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly IConfigFile _configFile;
        private readonly SyncNetConfiguration _configuration;
        private readonly IWindowManager _windowManager;

        public ConfigurationViewModel(SyncNetConfiguration configuration, IWindowManager windowManager,
            IConfigFile configFile)
        {
            _configuration = configuration;
            _windowManager = windowManager;
            _configFile = configFile;
            SelectFile = new RelayCommand(
                p => true,
                p => { LocalDirectory = _windowManager.ShowDirectoryDialog(); });

            Save = new RelayCommand(
                p => true,
                p =>
                {
                    using (var stream = _configFile.GetStream())
                    {
                        try
                        {
                            _configuration.Save(stream);
                            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                            Application.Current.Shutdown();
                        }
                        catch (Exception e)
                        {
                            _windowManager.ShowMessage(e.Message);
                        }
                    }
                });
        }

        public string ProfileName
        {
            get { return _configuration.ProfileName; }
            set { _configuration.ProfileName = value;
                OnPropertyChanged();
            }
        }

        public string KeySecret
        {
            get { return _configuration.KeySecret; }
            set { _configuration.KeySecret = value;
                OnPropertyChanged();
            }
        }

        public string KeyId
        {
            get { return _configuration.KeyId; }
            set { _configuration.KeyId = value;
                OnPropertyChanged();
            }
        }

        public CredentialsType CredentialsType
        {
            get { return _configuration.CredentialsType; }
            set { _configuration.CredentialsType = value;
                OnPropertyChanged();
            }
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
            set
            {
                _configuration.S3Bucket = value;
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

        public ICommand SelectFile { get; }

        public ICommand Save { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}