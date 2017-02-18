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
    public class CategoryTypesController : Controller
    {
        //private ExpenseTrackerContext db = new ExpenseTrackerContext();
        private ExpensesService.ExpenseTrackerServiceClient _serviceClient
            = new ExpensesService.ExpenseTrackerServiceClient();
        // GET: CategoryTypes
        public ActionResult Index()
        {
            return View(_serviceClient.GetCategoryTypes());
        }

        // GET: CategoryTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = _serviceClient.GetCategoryTypeById(id.Value);//db.Types.Find(id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            return View(categoryType);
        }

        // GET: CategoryTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdType,Name")] CategoryType categoryType)
        {
            if (ModelState.IsValid)
            {
                //db.Types.Add(categoryType);
                //db.SaveChanges();
                _serviceClient.AddCategoryType(categoryType);
                return RedirectToAction("Index");
            }

            return View(categoryType);
        }

        // GET: CategoryTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = _serviceClient.GetCategoryTypeById(id.Value);//db.Types.Find(id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            return View(categoryType);
        }

        // POST: CategoryTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdType,Name")] CategoryType categoryType)
        {
            if (ModelState.IsValid)
            {
                _serviceClient.UpdateCategoryType(categoryType);
                //db.Entry(categoryType).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoryType);
        }

        // GET: CategoryTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = _serviceClient.GetCategoryTypeById(id.Value);//db.Types.Find(id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            return View(categoryType);
        }

        // POST: CategoryTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _serviceClient.DeleteCategoryTypeById(id);
            //CategoryType categoryType = db.Types.Find(id);
            //db.Types.Remove(categoryType);
            //db.SaveChanges();
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
