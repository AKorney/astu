using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Model
{
    public interface IAccountRepository
    {
        void Add(Account account);
        void Edit(Account account);
        void Remove(int id);
        IEnumerable<Account> GetAccounts();
        Account FindById(int id);
    }
}
