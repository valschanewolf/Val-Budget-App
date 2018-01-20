using BudgetApp.Helpers;
using BudgetApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetApp.Controllers
{
	public class HomeController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		private ApplicationUser user = new ApplicationUser();
		private UserHelper userHelper = new UserHelper();
		private DashboardHelper dashboardHelper = new DashboardHelper();

		[Authorize]
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");

		}

		public ActionResult Dashboard()
		{
			return RedirectToAction(dashboardHelper.GetMyDashboard(), "Dashboard");
		}


		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}