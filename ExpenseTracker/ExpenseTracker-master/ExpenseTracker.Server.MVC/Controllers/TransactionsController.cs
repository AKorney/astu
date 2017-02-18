using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.Model;

namespace ExpenseTracker.Server.MVC.Controllers
{
    public class TransactionsController : Controller
    {
        //private ExpenseTrackerContext db = new ExpenseTrackerContext();
        private ExpensesService.ExpenseTrackerServiceClient _serviceClient
            = new ExpensesService.ExpenseTrackerServiceClient();
        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = _serviceClient.GetTransactions();//db.Transactions.Include(t => t.Account).Include(t => t.Category);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = _serviceClient.GetTransactionById(id.Value);//db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.FKAccount = new SelectList(_serviceClient.GetAccounts(), "IdAccount", "Name");
            ViewBag.FkCategory = new SelectList(_serviceClient.GetCategories(), "IdCategory", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTransaction,FKAccount,FkCategory,Name,Date,Notes")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                //db.Transactions.Add(transaction);
                //db.SaveChanges();
                _serviceClient.AddTransaction(transaction);
                return RedirectToAction("Index");
            }

            ViewBag.FKAccount = new SelectList(_serviceClient.GetAccounts(), "IdAccount", "Name", transaction.FKAccount);
            ViewBag.FkCategory = new SelectList(_serviceClient.GetCategories(), "IdCategory", "Name", transaction.FkCategory);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = _serviceClient.GetTransactionById(id.Value);//db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.FKAccount = new SelectList(_serviceClient.GetAccounts(), "IdAccount", "Name", transaction.FKAccount);
            ViewBag.FkCategory = new SelectList(_serviceClient.GetCategories(), "IdCategory", "Name", transaction.FkCategory);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTransaction,FKAccount,FkCategory,Name,Date,Notes")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(transaction).State = EntityState.Modified;
                //db.SaveChanges();
                _serviceClient.UpdateTransaction(transaction);
                return RedirectToAction("Index");
            }
            ViewBag.FKAccount = new SelectList(_serviceClient.GetAccounts(), "IdAccount", "Name", transaction.FKAccount);
            ViewBag.FkCategory = new SelectList(_serviceClient.GetCategories(), "IdCategory", "Name", transaction.FkCategory);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = _serviceClient.GetTransactionById(id.Value);//db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Transaction transaction = db.Transactions.Find(id);
            //db.Transactions.Remove(transaction);
            //db.SaveChanges();
            _serviceClient.DeleteTransactionById(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
                _serviceClient.Close();
            }
            base.Dispose(disposing);
        }
    }
}
