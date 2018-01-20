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
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BudgetApp.Controllers
{
	public class InvitationsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		private CommunicationHelper communicationHelper = new CommunicationHelper();
		private EmailHelper emailHelper = new EmailHelper();

		// GET: Invitations
		public ActionResult Index()
		{
			var invitations = db.Invitations.Include(i => i.Household);
			return View(invitations.ToList());
		}

		// GET: Invitations/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Invitation invitation = db.Invitations.Find(id);
			if (invitation == null)
			{
				return HttpNotFound();
			}
			return View(invitation);
		}

		// GET: Invitations/Create
		public ActionResult Create()
		{
			ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName");
			return View();
		}

		// POST: Invitations/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include = "Id,InvitationCode,Email,DateSent,Accepted,HouseholdId")] Invitation invitation)
		public async Task<ActionResult> Create([Bind(Include = "Email")] Invitation invitation)
		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				var householdId = user.HouseholdId.Value;
				invitation.HouseholdId = householdId;
				db.Invitations.Add(invitation);
				//string code = communicationHelper.GenerateInvitationCode();
				string code = Guid.NewGuid().ToString();
				invitation.InvitationCode = code;
				invitation.Accepted = false;
				invitation.DateSent = DateTime.Now;
				db.SaveChanges();
				var message = new IdentityMessage();
				message.Destination = invitation.Email;
				message.Subject = "Household Invite";
				var callbackUrl = Url.Action("RegisterByInvite", "Account", new { invitationEmail = invitation.Email, invitationCode = code }, protocol: Request.Url.Scheme);

				//var callbackUrl = Url.Action("RegisterByInvite", "Account", new { invitationId = invitation.Id, invitationEmail = invitation.Email, invitationCode = code }, protocol: Request.Url.Scheme);
				message.Body = "Please join a household by clicking <a href=\"" + callbackUrl + "\">here</a>";
				var emailSvc = new EmailService();
				await emailSvc.SendAsync(message);
				//await EmailHelper.SendInvite(invitation.Email, callbackUrl);

				//await UserManager.SendEmailAsync(invitation.HouseholdId, "Join a household", "Please join a household by clicking <a href=\"" + callbackUrl + "\">here</a>");
			
				return PartialView("Added", invitation);
				//return RedirectToAction("Index");
			}

			//ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", invitation.HouseholdId);
			return View();
		}

		// GET: Invitations/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Invitation invitation = db.Invitations.Find(id);
			if (invitation == null)
			{
				return HttpNotFound();
			}
			ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", invitation.HouseholdId);
			return View(invitation);
		}

		// POST: Invitations/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,InvitationCode,Email,DateSent,Accepted,HouseholdId")] Invitation invitation)
		{
			if (ModelState.IsValid)
			{
				db.Entry(invitation).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", invitation.HouseholdId);
			return View(invitation);
		}

		// GET: Invitations/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Invitation invitation = db.Invitations.Find(id);
			if (invitation == null)
			{
				return HttpNotFound();
			}
			return View(invitation);
		}

		// POST: Invitations/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Invitation invitation = db.Invitations.Find(id);
			db.Invitations.Remove(invitation);
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
