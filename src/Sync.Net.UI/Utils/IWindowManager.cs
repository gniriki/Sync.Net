﻿using Sync.Net.Processing;

namespace Sync.Net.UI.Utils
{
    public interface IWindowManager
    {
        string ShowDirectoryDialog();
        void ShutdownApplication();
        void ShowMessage(string message);
        void ShowConfiguration();
        void RestartApplication();
        void ShowTaskError(TaskQueueErrorEventArgs eventArgs);
    }
}