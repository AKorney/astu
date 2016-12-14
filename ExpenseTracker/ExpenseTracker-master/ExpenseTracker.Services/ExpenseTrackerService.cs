using ExpenseTracker.BusinessLogic;
using ExpenseTracker.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    [ServiceContract]
    public class ExpenseTrackerService
    {
        #region CategoryType
        [OperationContract]
        public IList<CategoryType> GetCategoryTypes()
        {
            using (var db = new ExpenseTrackerContext())
            {
                return db.Types.ToList();
            }
        }
        [OperationContract]
        public CategoryType GetCategoryTypeById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                var categoryType = db.Types.Find(id);
                return categoryType;
            }
        }
        [OperationContract]
        public void AddCategoryType(CategoryType categoryType)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Types.Add(categoryType);
                db.SaveChanges();   
            }
        }

        [OperationContract]
        public void UpdateCategoryType(CategoryType categoryType)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Entry(categoryType).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteCategoryTypeById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                var categoryType = db.Types.Find(id);
                db.Types.Remove(categoryType);
                db.SaveChanges();
            }
        }
        #endregion

        #region Category
        [OperationContract]
        public IList<Category> GetCategories()
        {
            using (var db = new ExpenseTrackerContext())
            {
                var categories = db.Categories.Include(c => c.Type);
                return categories.ToList();
            }
        }

        [OperationContract]
        public Category GetCategoryById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                var category = db.Categories.Find(id);
                return category;
            }
        }

        [OperationContract]
        public void AddCategory(Category category)
        {
            using(var db = new ExpenseTrackerContext())
            {
                db.Categories.Add(category);
                db.SaveChanges();
            }
        }
        [OperationContract]
        public void UpdateCategory(Category category)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        [OperationContract]
        public void DeleteCategoryById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
            }
        }
        #endregion

        #region Transaction
        [OperationContract]
        public IList<Transaction> GetTransactions()
        {
            using (var db = new ExpenseTrackerContext())
            {
                var transactions = db.Transactions.Include(t => t.Account).Include(t => t.Category);
                return transactions.ToList();
            }
        }

        [OperationContract]
        public Transaction GetTransactionById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                Transaction transaction = db.Transactions.Find(id);
                return transaction;
            }
        }

        [OperationContract]
        public void AddTransaction(Transaction transaction)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }
        }
        [OperationContract]
        public void UpdateTransaction(Transaction transaction)
        {
            using(var db = new ExpenseTrackerContext())
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteTransactionById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                Transaction transaction = db.Transactions.Find(id);
                db.Transactions.Remove(transaction);
                db.SaveChanges();
            }
        }
        #endregion

        #region Account
        [OperationContract]
        public IList<Account> GetAccounts()
        {
            using (var db = new ExpenseTrackerContext())
            {
                return db.Accounts.ToList();
            }
        }

        [OperationContract]
        public Account GetAccountById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                Account account = db.Accounts.Find(id);
                return account;
            }
        }
        [OperationContract]
        public void AddAccount(Account account)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Accounts.Add(account);
                db.SaveChanges();
            }
        }
        [OperationContract]
        public void UpdateAccount(Account account)
        {
            using (var db = new ExpenseTrackerContext())
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteAccountById(int id)
        {
            using (var db = new ExpenseTrackerContext())
            {
                Account account = db.Accounts.Find(id);
                db.Accounts.Remove(account);
                db.SaveChanges();
            }
        }
        #endregion
    }
}
