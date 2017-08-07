using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Sync.Net.UI.ViewModels;

namespace Sync.Net.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContextChanged += MainWindow_DataContextChanged;
            InitializeComponent();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var mainWindowViewModel = e.NewValue as MainWindowViewModel;

            if (mainWindowViewModel != null)
                mainWindowViewModel.PropertyChanged += MainWindow_PropertyChanged;
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Log))
                textBox.Dispatcher.Invoke(() => textBox.ScrollToEnd());
        }
    }
}