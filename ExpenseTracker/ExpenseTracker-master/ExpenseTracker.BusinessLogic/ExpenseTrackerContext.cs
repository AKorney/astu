using ExpenseTracker.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.BusinessLogic
{
    public class ExpenseTrackerContext:DbContext
    {

        public ExpenseTrackerContext() : base("name=ExpenseTracker")
        {
        }        
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<CategoryType> Types { get; set; }

    }
}
