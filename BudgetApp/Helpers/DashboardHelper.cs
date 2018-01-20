using BudgetApp.Helpers;
using BudgetApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetApp.Helpers
{
	public class DashboardHelper
	{
		//private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
		//private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
		private ApplicationDbContext db = new ApplicationDbContext();
		private UserHelper userHelper = new UserHelper();

		public string GetMyDashboard ()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var userRole = userHelper.GetCurrentUserRole(userId);
			switch (userRole)
			{
				case "GuestAtFrontDoor":
					return "DashboardGuestAtFrontDoor";
				case "HeadOfHousehold":
					return "DashboardHeadOfHousehold";
				case "MemberOfHousehold":
					return "DashboardMemberOfHousehold";
				case "Administrator":
				case "SuperUser":
					return "DashboardAdministrator";
				default:
					return "ErrorPage";
			}
		}
	}
}