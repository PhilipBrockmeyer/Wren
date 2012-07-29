using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Wren
{
    public class ActionCommand : ICommand
    {
        Action<Object> _action;
        Func<Object, Boolean> _predicate;

        public ActionCommand(Action<Object> executeAction, Func<Object, Boolean> canExecutePredicate)
        {
            _action = executeAction;
            _predicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
    }
}
