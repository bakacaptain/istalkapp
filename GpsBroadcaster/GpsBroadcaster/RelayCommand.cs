using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GpsBroadcaster
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<Object> _execute;
        private readonly Predicate<Object> _canExecute;

        #endregion

        #region Constructors

        public RelayCommand(Action<Object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<Object> execute, Predicate<Object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(Object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
