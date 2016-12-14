using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpenseTracker
{
    public class AccountsViewModel:Observable
    {
        public AccountsViewModel()
        {
            AddAccountCommand = new Command<object>(() => AddAccount());
        }

        private void AddAccount()
        {

            
        }

        public ICommand AddAccountCommand
        {
            get;
            set;
        }
    }
}
