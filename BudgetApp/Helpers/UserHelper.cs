using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BudgetApp.Models;
using System.Data.Entity;

namespace BudgetApp.Helpers
{
	public class UserHelper
	{
		private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
		private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
		private static ApplicationDbContext db = new ApplicationDbContext();

		public string GetCurrentUserRole(string userId)
		{
			var CurrentRole = userManager.GetRoles(userId).FirstOrDefault();
			return CurrentRole;
		}

		public static string GetCurrentUserName()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			if (userId == null)
				return "#";
			else
			{
				var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
				return user.Email;
			}
		}

		public bool IsUserInRole(string userId, string roleName)
		{
			return userManager.IsInRole(userId, roleName);
		}

		public bool AddUserToRole(string userId, string roleName)
		{
			var result = userManager.AddToRole(userId, roleName);
			return result.Succeeded;
		}

		public bool RemoveUserFromRole(string userId, string roleName)
		{
			var result = userManager.RemoveFromRole(userId, roleName);
			return result.Succeeded;
		}

		public bool IsUserInAHousehold(ApplicationUser user)
		{
			if (user.HouseholdId != null)
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

	}
}