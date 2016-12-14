using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker
{
    public class Command<TParameterType> : CommandBase, IUpdatableCommand
    {
        private Action<TParameterType> _executeAction = null;
        private TParameterType _defaultValue;

        public Command(Action<TParameterType> executeAction, Func<bool> canExecute, TParameterType defaultValue)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
            _defaultValue = defaultValue;
        }
        public Command(Action<TParameterType> executeAction, TParameterType defaultValue)
        {
            _executeAction = executeAction;
            _canExecute = _defaultCanExecute;
            _defaultValue = defaultValue;
        }

        public Command(Action executeAction, Func<bool> canExecute)
        {
            _executeAction = delegate { executeAction.Invoke(); };
            _canExecute = canExecute;
        }

        public Command(Action executeAction)
        {
            _executeAction = delegate { executeAction.Invoke(); };
            _canExecute = _defaultCanExecute;
        }

        public Command()
        {
            _canExecute = _defaultCanExecute;
        }

        internal void UpdateCanExecute(Func<bool> canExecute)
        {
            _canExecute = canExecute;
            RaiseCanExecuteChanged();
        }

        #region IUpdatableCommand Members

        public void UpdateCanExecute()
        {
            RaiseCanExecuteChanged();
        }

        public void SetIsSupported(bool value)
        {
            IsSupported = value;
        }

        #endregion

        #region IExtendedCommand Members

        public bool GetIsSupported()
        {
            return IsSupported;
        }

        #endregion

        public override void Execute(object parameter)
        {
            if (parameter == null)
            {
                _executeAction.Invoke(_defaultValue);
            }
            else
            {
                TParameterType v = (TParameterType)parameter;
                _executeAction.Invoke(v);
            }
        }
    }
}
