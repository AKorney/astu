using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpenseTracker
{
    public abstract class CommandBase : ICommand
    {
        protected Func<bool> _canExecute = null;
        protected bool _canExecuteCache = true;
        protected Func<bool> _defaultCanExecute = delegate { return true; };

        private bool _isSupported;

        public bool IsSupported
        {
            get { return _isSupported; }
            protected set { _isSupported = value; }
        }

        private Boolean _isEnabled = true;

        public Boolean IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaiseCanExecuteChanged();
                }
            }
        }
        /// <summary>
        /// Defines the method that determines whether the command 
        /// can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public virtual bool CanExecute(object parameter)
        {
            if (!_isEnabled)
            {
                return false;
            }
            bool tempCanExecute = _canExecute();
            if (_canExecuteCache != tempCanExecute)
            {
                _canExecuteCache = tempCanExecute;
                RaiseCanExecuteChanged();
            }
            return _canExecuteCache;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter) { }

        public void RaiseCanExecuteChanged()
        {
            try
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            // Workaround to avoid unexpected COMException in some cases
            catch (Exception)
            {
            }
        }

        public CommandBase()
        {
            _canExecute = _defaultCanExecute;
        }
    }

}
