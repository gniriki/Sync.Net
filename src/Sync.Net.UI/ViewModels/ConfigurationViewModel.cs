using System;
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
        private readonly IWindowManager _windowManager;
        private readonly IConfigurationTester _configurationTester;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IProcessorConfigurationValidator _validator;

        public ConfigurationViewModel(IConfigurationProvider configurationProvider, IWindowManager windowManager, IConfigFile configFile, IConfigurationTester configurationTester, IProcessorConfigurationValidator validator)
        {
            _configurationProvider = configurationProvider;
            _windowManager = windowManager;
            _configFile = configFile;
            _configurationTester = configurationTester;
            SelectFile = new RelayCommand(
                p => true,
                p => { LocalDirectory = _windowManager.ShowDirectoryDialog(); });

            Save = new RelayCommand(
                p => true,
                p => { SaveConfiguration(); });

            Test = new RelayCommand(
                p => true,
                p => { CheckIfConfigurationIsValid(true); });
            _validator = validator;
        }

        private void SaveConfiguration()
        {
            bool canSave = CheckIfConfigurationIsValid(false);

            if (canSave)
            {
                _configFile.Clear();
                try
                {
                    _configurationProvider.Save();
                    _windowManager.RestartApplication();
                }
                catch (Exception e)
                {
                    _windowManager.ShowMessage(e.Message);
                }
            }
        }

        private bool CheckIfConfigurationIsValid(bool showConfirmationMessage)
        {
            var validationResults = _validator.Validate(_configurationProvider.Current);
            if (!validationResults.IsValid)
            {
                _windowManager.ShowMessage(validationResults.Message);
                return false;
            }

            var testResults = _configurationTester.Test(_configurationProvider.Current);

            if (!testResults.TestPassed)
            {
                _windowManager.ShowMessage(testResults.Message);
            }
            else if (showConfirmationMessage)
                _windowManager.ShowMessage("Ok");
            return testResults.TestPassed;
        }

        public string ProfileName
        {
            get { return _configurationProvider.Current.ProfileName; }
            set
            {
                _configurationProvider.Current.ProfileName = value;
                OnPropertyChanged();
            }
        }

        public string KeySecret
        {
            get { return _configurationProvider.Current.KeySecret; }
            set
            {
                _configurationProvider.Current.KeySecret = value;
                OnPropertyChanged();
            }
        }

        public string KeyId
        {
            get { return _configurationProvider.Current.KeyId; }
            set
            {
                _configurationProvider.Current.KeyId = value;
                OnPropertyChanged();
            }
        }

        public CredentialsType? CredentialsType
        {
            get { return _configurationProvider.Current.CredentialsType; }
            set
            {
                _configurationProvider.Current.CredentialsType = value;
                OnPropertyChanged();
                OnPropertyChanged($"CredentialsTypeSet");
            }
        }

        public bool CredentialsTypeSet => CredentialsType.HasValue;

        public string LocalDirectory
        {
            get { return _configurationProvider.Current.LocalDirectory; }
            set
            {
                _configurationProvider.Current.LocalDirectory = value;
                OnPropertyChanged();
            }
        }

        public string S3Bucket
        {
            get { return _configurationProvider.Current.S3Bucket; }
            set
            {
                _configurationProvider.Current.S3Bucket = value;
                OnPropertyChanged();
            }
        }

        public RegionEndpoint RegionEndpoint
        {
            get { return _configurationProvider.Current.RegionEndpoint; }
            set
            {
                _configurationProvider.Current.RegionEndpoint = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectFile { get; }

        public ICommand Save { get; }

        public ICommand Test { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}