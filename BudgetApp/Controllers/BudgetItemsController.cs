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
	public class BudgetItemsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: BudgetItems
		public ActionResult Index()
		{
			//var budgetItems = db.BudgetItems.Include(b => b.Budget).Include(b => b.BudgetItemType);
			var budgetItems = db.BudgetItems.Include(b => b.Budget);

			return View(budgetItems.ToList());
		}

		// GET: BudgetItems/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BudgetItem budgetItem = db.BudgetItems.Find(id);
			if (budgetItem == null)
			{
				return HttpNotFound();
			}
			return View(budgetItem);
		}

		// GET: BudgetItems/Create
		public ActionResult Create()
		{
			ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
			//ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "ItemType");
			return View();
		}

		// POST: BudgetItems/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include = "Id,Name,Description,BudgetId,BudgetItemTypeId")] BudgetItem budgetItem)
		public ActionResult Create([Bind(Include = "BudgetName,ItemAmount")] BudgetItem budgetItem, Budget budget, BudgetItemType budgetItemType)

		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				budget.HouseholdId = user.HouseholdId.Value;
				if (budget.BudgetName != "Select")
				{
					var index = budget.BudgetName.IndexOf("/");
					var itemCategory = budget.BudgetName.Substring(0, index);
					var itemType = budget.BudgetName.Substring(index + 1);
					budget.BudgetName = itemCategory;
					db.Budgets.Add(budget);
					db.SaveChanges();
					budgetItemType.ItemCategory = itemCategory;
					budgetItemType.ItemType = itemType;
					db.BudgetItemTypes.Add(budgetItemType);
					db.SaveChanges();
					budgetItem.ItemName = itemType;
					budgetItem.StartDate = DateTime.Now;
					budgetItem.BudgetId = budget.Id;
					budgetItem.BudgetItemTypeId = budgetItemType.Id;
					db.BudgetItems.Add(budgetItem);
					db.SaveChanges();
					var addedBudgets = db.Budgets.Where(h => h.HouseholdId == budget.HouseholdId).Include(i => i.BudgetItems).OrderBy(c => c.BudgetName).ToList();
					return PartialView("Added", addedBudgets);
				}
			}
			return View();
		}

		// POST: BudgetItems/CreateCustom
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include = "Id,Name,Description,BudgetId,BudgetItemTypeId")] BudgetItem budgetItem)
		public ActionResult CreateCustom([Bind(Include = "ItemType,ItemCategory,ItemAmount")] BudgetItem budgetItem, Budget budget, BudgetItemType budgetItemType)
		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				budget.HouseholdId = user.HouseholdId.Value;

				budget.BudgetName = budgetItemType.ItemCategory;
				db.Budgets.Add(budget);
				db.SaveChanges();
				db.BudgetItemTypes.Add(budgetItemType);
				db.SaveChanges();
				budgetItem.ItemName = budgetItemType.ItemType;
				budgetItem.StartDate = DateTime.Now;
				budgetItem.BudgetId = budget.Id;
				budgetItem.BudgetItemTypeId = budgetItemType.Id;
				db.BudgetItems.Add(budgetItem);
				db.SaveChanges();
				var addedBudgets = db.Budgets.Where(h => h.HouseholdId == budget.HouseholdId).Include(i => i.BudgetItems).OrderBy(c => c.BudgetName).ToList();
				return PartialView("Added", addedBudgets);
			}
			return View();
		}

		// GET: BudgetItems/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BudgetItem budgetItem = db.BudgetItems.Find(id);
			if (budgetItem == null)
			{
				return HttpNotFound();
			}
			ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
			//ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "ItemType", budgetItem.BudgetItemTypeId);
			return View(budgetItem);
		}

		// POST: BudgetItems/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,Description,BudgetId,BudgetItemTypeId")] BudgetItem budgetItem)
		{
			if (ModelState.IsValid)
			{
				db.Entry(budgetItem).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
			//ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "ItemType", budgetItem.BudgetItemTypeId);
			return View(budgetItem);
		}

		// GET: BudgetItems/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BudgetItem budgetItem = db.BudgetItems.Find(id);
			if (budgetItem == null)
			{
				return HttpNotFound();
			}
			return View(budgetItem);
		}

		// POST: BudgetItems/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			BudgetItem budgetItem = db.BudgetItems.Find(id);
			db.BudgetItems.Remove(budgetItem);
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
