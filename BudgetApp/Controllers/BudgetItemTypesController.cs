using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BudgetApp.Models;

namespace BudgetApp.Controllers
{
    public class BudgetItemTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItemTypes
        public ActionResult Index()
        {
            return View(db.BudgetItemTypes.ToList());
        }

        // GET: BudgetItemTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemType budgetItemType = db.BudgetItemTypes.Find(id);
            if (budgetItemType == null)
            {
                return HttpNotFound();
            }
            return View(budgetItemType);
        }

        // GET: BudgetItemTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BudgetItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemType")] BudgetItemType budgetItemType)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItemTypes.Add(budgetItemType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(budgetItemType);
        }

        // GET: BudgetItemTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemType budgetItemType = db.BudgetItemTypes.Find(id);
            if (budgetItemType == null)
            {
                return HttpNotFound();
            }
            return View(budgetItemType);
        }

        // POST: BudgetItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemType")] BudgetItemType budgetItemType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItemType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budgetItemType);
        }

        // GET: BudgetItemTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemType budgetItemType = db.BudgetItemTypes.Find(id);
            if (budgetItemType == null)
            {
                return HttpNotFound();
            }
            return View(budgetItemType);
        }

        // POST: BudgetItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItemType budgetItemType = db.BudgetItemTypes.Find(id);
            db.BudgetItemTypes.Remove(budgetItemType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
