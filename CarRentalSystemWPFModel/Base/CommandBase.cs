using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Base.WPFModel
{
    public class CommandBase : ICommand
    {
        private Func<object, bool> _canExecute;
        private Action<object> _executeAction;
        private bool canExecuteCache;

        public CommandBase(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this._executeAction = executeAction;
            this._canExecute = canExecute;
        }
        public ICommand Command
        {
            set
            {

            }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            bool tempCanExecute = _canExecute(parameter);
            canExecuteCache = tempCanExecute;
            return canExecuteCache;
        }
        private event EventHandler _canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add { this._canExecuteChanged += value; }
            remove { this._canExecuteChanged -= value; }
        }
        protected virtual void OnCanExecuteChanged()
        {
            if (this._canExecuteChanged != null)
                this._canExecuteChanged(this, EventArgs.Empty);
        }
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        #endregion
    }
}
