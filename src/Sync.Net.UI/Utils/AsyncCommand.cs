using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sync.Net.UI.Utils
{
    public class AsyncCommand : ICommand
    {
        public AsyncCommand(Func<Task> executedHandler, Func<bool> canExecuteHandler = null)
        {
            if (executedHandler == null)
                throw new ArgumentNullException(nameof(executedHandler));

            ExecutedHandler = executedHandler;
            CanExecuteHandler = canExecuteHandler;
        }

        public Func<Task> ExecutedHandler { get; }

        public Func<bool> CanExecuteHandler { get; }
        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        async void ICommand.Execute(object parameter)
        {
            await Execute();
        }

        public Task Execute()
        {
            return ExecutedHandler();
        }

        public bool CanExecute()
        {
            return CanExecuteHandler == null || CanExecuteHandler();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}