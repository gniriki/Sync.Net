using System;
using System.Windows;
using Ookii.Dialogs.Wpf;
using Sync.Net.Processing;

namespace Sync.Net.UI.Utils
{
    public class WindowManager : IWindowManager
    {
        public string ShowDirectoryDialog()
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                return dialog.SelectedPath;
            return null;
        }

        public void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        public void RestartApplication()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void ShowTaskError(TaskQueueErrorEventArgs eventArgs)
        {
            var result = MessageBox
                .Show($"Error while processing task {eventArgs.Task}.\nDetails: {eventArgs.ThrownException}\nRetry?", "Error", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    eventArgs.TaskQueueErrorResponse = TaskQueueErrorResponse.Abort;
                    break;
                case MessageBoxResult.Yes:
                    eventArgs.TaskQueueErrorResponse = TaskQueueErrorResponse.Retry;
                    break;
                case MessageBoxResult.No:
                    eventArgs.TaskQueueErrorResponse = TaskQueueErrorResponse.Skip;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowConfiguration()
        {
            var window = new Window {Content = new Configuration()};
            window.Show();
        }
    }
}