using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BudgetApp.Models;
using BudgetApp.Helpers;
using Microsoft.AspNet.Identity;

namespace BudgetApp.Controllers
{
	public class HouseholdsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		private ApplicationUser user = new ApplicationUser();
		private UserHelper userHelper = new UserHelper();
		private HouseholdHelper householdHelper = new HouseholdHelper();

		// GET: Households
		public ActionResult Index()
		{
			return View(db.Households.ToList());
		}

		// GET: Households/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Household household = db.Households.Find(id);
			if (household == null)
			{
				return HttpNotFound();
			}
			return View(household);
		}

		// GET: Households/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Households/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,HouseholdName")] Household household)
		{
			if (ModelState.IsValid)
			{
				//var userId = User.Identity.GetUserId();
				var user = db.Users.Find(User.Identity.GetUserId());

				if (user.HouseholdId == null)
				{
					db.Households.Add(household);
					userHelper.AddUserToRole(user.Id, "HeadOfHousehold");
					household.Members.Add(user);
					household.DateCreated = DateTime.Now;
					household.Active = true;
					db.SaveChanges();
				}
				else
				{
					TempData["alert"] = "You're already in a household";
					return RedirectToAction("Create");

					//send error "You're already in a household..."
				}
				return RedirectToAction("Index");
			}

			return View(household);
		}

		// GET: Households/Join/5
		public ActionResult Join(int invitationId, string invitationEmail, string code)
		{
			var invitation = db.Invitations.FirstOrDefault(i => i.Id == invitationId);
			Household household = db.Households.Find(invitation.HouseholdId);
			if (invitation.InvitationCode == code)
			{
				ViewBag.HouseholdName = household.HouseholdName;
				ViewBag.InviteEmail = invitationEmail;
				ViewBag.Code = code;
				return View(household);
			}
			else
			{
				return HttpNotFound();
			}
		}

		// POST: Households/Join/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Join([Bind(Include = "Id,HouseholdName,Active,DateCreated")] Household household)
		public ActionResult Join(Household household)
		{
			if (ModelState.IsValid)
			{
				//db.Entry(household).State = EntityState.Modified;
				//db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(household);
		}


		// GET: Households/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Household household = db.Households.Find(id);
			if (household == null)
			{
				return HttpNotFound();
			}
			return View(household);
		}

		// POST: Households/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,HouseholdName,Active,DateCreated")] Household household)
		{
			if (ModelState.IsValid)
			{
				db.Entry(household).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(household);
		}

		// GET: Households/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Household household = db.Households.Find(id);
			if (household == null)
			{
				return HttpNotFound();
			}
			return View(household);
		}

		// POST: Households/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Household household = db.Households.Find(id);
			db.Households.Remove(household);
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
