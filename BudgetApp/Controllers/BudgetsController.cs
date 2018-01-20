using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BudgetApp.Models;
using Microsoft.AspNet.Identity;

namespace BudgetApp.Controllers
{
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var budgets = db.Budgets.Include(b => b.Household);
            return View(budgets.ToList());
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName");
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include = "Id,Name,BudgetAmount,WarningAmount,HouseholdId")] Budget budget)
		public ActionResult Create([Bind(Include = "BudgetName,ItemAmount")] Budget budget, BudgetItem budgetItem)
		{
			if (ModelState.IsValid)
            {
				var user = db.Users.Find(User.Identity.GetUserId());
				//var householdId = user.HouseholdId;
				budget.HouseholdId = user.HouseholdId.Value;
				var index = budget.BudgetName.IndexOf("/");
				var itemCategory = budget.BudgetName.Substring(0, index);
				var itemType = budget.BudgetName.Substring(index + 1);
				budget.BudgetName = itemCategory;
				db.Budgets.Add(budget);
                db.SaveChanges();
				budgetItem.ItemName = itemType;
				budgetItem.StartDate = DateTime.Now;
				budgetItem.BudgetId = budget.Id;
				db.BudgetItems.Add(budgetItem);
				db.SaveChanges();
				var addedBudgets = db.BudgetItems.AsNoTracking().Where(i => i.Budget.HouseholdId == user.HouseholdId.Value).ToList();
				return PartialView("Added", addedBudgets);
                //return RedirectToAction("Index");
            }

            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,BudgetAmount,WarningAmount,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
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
