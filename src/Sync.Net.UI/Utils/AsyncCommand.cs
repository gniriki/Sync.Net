using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sync.Net.UI.Utils
{
    public class AsyncCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Func<Task> ExecutedHandler { get; }

        public Func<bool> CanExecuteHandler { get; }

        public AsyncCommand(Func<Task> executedHandler, Func<bool> canExecuteHandler = null)
        {
            if (executedHandler == null)
            {
                throw new ArgumentNullException(nameof(executedHandler));
            }

            this.ExecutedHandler = executedHandler;
            this.CanExecuteHandler = canExecuteHandler;
        }

        public Task Execute()
        {
            return this.ExecutedHandler();
        }

        public bool CanExecute()
        {
            return this.CanExecuteHandler == null || this.CanExecuteHandler();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        async void ICommand.Execute(object parameter)
        {
            await this.Execute();
        }
    }
}
