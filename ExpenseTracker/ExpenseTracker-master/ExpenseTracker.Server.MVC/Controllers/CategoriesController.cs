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
    public class CategoriesController : Controller
    {
        //private ExpenseTrackerContext db = new ExpenseTrackerContext();
        private ExpensesService.ExpenseTrackerServiceClient _serviceClient
            = new ExpensesService.ExpenseTrackerServiceClient();
        // GET: Categories
        public ActionResult Index()
        {
            var categories = _serviceClient.GetCategories();
            return View(categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _serviceClient.GetCategoryById(id.Value);//db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            ViewBag.FKType = new SelectList(_serviceClient.GetCategoryTypes(),//db.Types, 
                "IdType", "Name");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCategory,FKType,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                //db.Categories.Add(category);
                //db.SaveChanges();
                _serviceClient.AddCategory(category);
                return RedirectToAction("Index");
            }

            ViewBag.FKType = new SelectList(_serviceClient.GetCategoryTypes(),//db.Types, 
                "IdType", "Name", category.FKType);
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _serviceClient.GetCategoryById(id.Value);//db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.FKType = new SelectList(_serviceClient.GetCategoryTypes(),//db.Types, 
                "IdType", "Name", category.FKType);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCategory,FKType,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(category).State = EntityState.Modified;
                //db.SaveChanges();
                _serviceClient.UpdateCategory(category);
                return RedirectToAction("Index");
            }
            ViewBag.FKType = new SelectList(_serviceClient.GetCategoryTypes(),//db.Types, 
                "IdType", "Name", category.FKType);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _serviceClient.GetCategoryById(id.Value);//db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Category category = db.Categories.Find(id);
            //db.Categories.Remove(category);
            //db.SaveChanges();
            _serviceClient.DeleteCategoryById(id);
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
