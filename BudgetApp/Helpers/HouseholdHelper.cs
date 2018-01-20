using BudgetApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetApp.Helpers
{
	public class HouseholdHelper
	{
		private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
		private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
		private ApplicationDbContext db = new ApplicationDbContext();
		private ApplicationUser user = new ApplicationUser();

		public List<ApplicationUser> ListHouseholdMembers(int householdId)
		{
			return db.Users.Where(u => u.HouseholdId == householdId).ToList();
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

		public SelectList CreateBudgetItemsSelectList()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var budgetItemsDictionary = new Dictionary<string, string>();
			var budgetItems = db.BudgetItems.Where(i => i.Budget.HouseholdId == householdId).Include(i => i.BudgetItemType).OrderBy(i => i.BudgetItemType.ItemCategory).ThenBy(i => i.ItemName).ToList();
			//var budgetCategories = budgetItems.Where(i => i.BudgetItemType.Id == i.BudgetItemTypeId).Distinct().ToList();
			SelectList currentBudgetItems = new SelectList(budgetItems, "Id", "ItemName", "BudgetItemType.ItemCategory", null, null);
			return currentBudgetItems;
		}

		public SelectList CreateBankAccountsSelectList()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var bankAccountList = db.BankAccounts.Where(h => h.HouseholdId == householdId).ToList();
			SelectList currentBankAccounts = new SelectList(bankAccountList.OrderBy(n => n.AccountName), "Id", "AccountName");
			return currentBankAccounts;
		}

		public Dictionary<string, decimal> GetCurrentBudgetExpenseSums()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var currentBudgetExpenseSums = new Dictionary<string, decimal>();
			var budgetExpenseList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName != "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetItemList = budgetExpenseList.SelectMany(i => i.BudgetItems).ToList();
			foreach (var item in budgetExpenseList)
			{
				decimal currentBudgetTotal = budgetItemList.Where(i => i.BudgetId == item.Id).Sum(i => i.ItemCurrentAmount);
				currentBudgetExpenseSums.Add(item.BudgetName, currentBudgetTotal);
			}
			return currentBudgetExpenseSums;
		}

		public Dictionary<string, decimal> GetCurrentBudgetIncomeSums()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var currentBudgetIncomeSums = new Dictionary<string, decimal>();
			var budgetIncomeList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName == "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetItemList = budgetIncomeList.SelectMany(i => i.BudgetItems).ToList();
			foreach (var item in budgetIncomeList)
			{
				decimal currentBudgetTotal = budgetItemList.Where(i => i.BudgetId == item.Id).Sum(i => i.ItemCurrentAmount);
				currentBudgetIncomeSums.Add(item.BudgetName, currentBudgetTotal);
			}
			return currentBudgetIncomeSums;
		}

		public decimal GetCurrentBudgetExpenseTotal()
		{
			Dictionary<string, decimal> currentBudgetExpenseSums = GetCurrentBudgetExpenseSums();
			decimal currentBudgetExpenseTotal = currentBudgetExpenseSums.Sum(i => i.Value);
			return currentBudgetExpenseTotal;
		}

		public decimal GetCurrentBudgetIncomeTotal()
		{
			Dictionary<string, decimal> currentBudgetIncomeSums = GetCurrentBudgetIncomeSums();
			decimal currentBudgetIncomeTotal = currentBudgetIncomeSums.Sum(i => i.Value);
			return currentBudgetIncomeTotal;
		}

		public Dictionary<string, decimal> GetCurrentBudgetExpensePercentages()
		{
			Dictionary<string, decimal> currentBudgetExpensePercentages = new Dictionary<string, decimal>();
			var budgetExpenseSums = GetBudgetExpenseSums();
			foreach (var currentSum in GetCurrentBudgetExpenseSums())
			{
				string budgetExpenseName = currentSum.Key;
				decimal budgetExpensePercent = currentSum.Value / budgetExpenseSums.FirstOrDefault(i => i.Key == currentSum.Key).Value;
				currentBudgetExpensePercentages.Add(budgetExpenseName, budgetExpensePercent);
			}
			currentBudgetExpensePercentages.Add("Expense Percent", GetCurrentBudgetExpenseTotal() / GetBudgetExpenseTotal());
			return currentBudgetExpensePercentages;
		}

		public Dictionary<string, decimal> GetCurrentBudgetIncomePercentages()
		{
			Dictionary<string, decimal> currentBudgetIncomePercentages = new Dictionary<string, decimal>();
			var budgetIncomeSums = GetBudgetIncomeSums();
			foreach (var currentSum in GetCurrentBudgetIncomeSums())
			{
				string budgetIncomeName = currentSum.Key;
				decimal budgetIncomePercent = currentSum.Value / budgetIncomeSums.FirstOrDefault(i => i.Key == currentSum.Key).Value;
				currentBudgetIncomePercentages.Add(budgetIncomeName, budgetIncomePercent);
			}
			currentBudgetIncomePercentages.Add("Income Percent", GetCurrentBudgetIncomeTotal() / GetBudgetIncomeTotal());
			return currentBudgetIncomePercentages;
		}

		public Dictionary<string, decimal> GetBudgetExpenseSums()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var budgetExpenseSums = new Dictionary<string, decimal>();
			var budgetExpenseList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName != "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetItemList = budgetExpenseList.SelectMany(i => i.BudgetItems).ToList();
			foreach (var item in budgetExpenseList)
			{
				decimal budgetTotal = budgetItemList.Where(i => i.BudgetId == item.Id).Sum(i => i.ItemAmount);
				budgetExpenseSums.Add(item.BudgetName, budgetTotal);
			}
			return budgetExpenseSums;
		}

		public Dictionary<string, decimal> GetBudgetIncomeSums()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var budgetIncomeSums = new Dictionary<string, decimal>();
			var budgetIncomeList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName == "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetItemList = budgetIncomeList.SelectMany(i => i.BudgetItems).ToList();
			foreach (var item in budgetIncomeList)
			{
				decimal budgetTotal = budgetItemList.Where(i => i.BudgetId == item.Id).Sum(i => i.ItemAmount);
				budgetIncomeSums.Add(item.BudgetName, budgetTotal);
			}
			return budgetIncomeSums;
		}

		public decimal GetBudgetExpenseTotal()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var budgetExpenseList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName != "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetExpenseTotal = budgetExpenseList.Sum(i => i.BudgetAmount);
			return budgetExpenseTotal;
		}

		public decimal GetBudgetIncomeTotal()
		{
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			var budgetIncomeList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).Where(i => i.BudgetName == "Income").OrderBy(c => c.BudgetName).ToList();
			var budgetIncomeTotal = budgetIncomeList.Sum(i => i.BudgetAmount);
			return budgetIncomeTotal;
		}

		public Dictionary<string, decimal> GetBudgetPercentages()
		{
			Dictionary<string, decimal> budgetPercentages = new Dictionary<string, decimal>();
			decimal budgetExpenseTotal = GetBudgetExpenseTotal();
			decimal budgetIncomeTotal = GetBudgetIncomeTotal();
			foreach (var expense in GetBudgetExpenseSums())
			{
				string budgetName = expense.Key;
				decimal budgetPercent = expense.Value / budgetExpenseTotal;
				budgetPercentages.Add(budgetName, budgetPercent);
			}
			foreach (var income in GetBudgetIncomeSums())
			{
				string budgetName = income.Key;
				decimal budgetPercent = income.Value / budgetIncomeTotal;
				budgetPercentages.Add(budgetName, budgetPercent);
			}
			return budgetPercentages;
		}

		public List<BudgetItem> CreateNewBudget(string name, decimal amount)
		{
			Budget budget = new Budget();
			BudgetItem budgetItem = new BudgetItem();
			BudgetItemType budgetItemType = new BudgetItemType();
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			DateTime now = DateTime.Now;

			if (name != "Select")
			{
				var index = name.IndexOf("/");
				var budgetName = name.Substring(0, index);
				var itemName = name.Substring(index + 1);
				if (db.Budgets.Where(h => h.HouseholdId == householdId).ToList().Any(i => i.BudgetName.ToLower().Replace(" ", "") == budgetName.ToLower().Replace(" ", "")))
				{
					budget = db.Budgets.Where(h => h.HouseholdId == householdId).FirstOrDefault(b => b.BudgetName.ToLower() == budgetName.ToLower());
					budgetItemType = db.BudgetItemTypes.FirstOrDefault(n => n.ItemCategory == budget.BudgetName);
				}
				else
				{
					budgetItemType.ItemCategory = budgetName;
					if (budgetName == "Income")
					{
						budget.BudgetType = "Income";
						budgetItemType.ItemType = "Income";
					}
					else
					{
						budget.BudgetType = "Expense";
						budgetItemType.ItemType = "Expense";
					}
					db.BudgetItemTypes.Add(budgetItemType);
					db.SaveChanges();
					budget.HouseholdId = householdId;
					budget.BudgetName = budgetName;
					db.Budgets.Add(budget);
					db.SaveChanges();
				}

				budgetItem.ItemName = itemName;
				budgetItem.StartDate = now;
				budgetItem.BudgetId = budget.Id;
				budgetItem.ItemAmount = amount;
				budgetItem.BudgetItemTypeId = budgetItemType.Id;
				db.BudgetItems.Add(budgetItem);
				db.SaveChanges();
				budget.BudgetAmount += amount;
				db.Entry(budget).State = EntityState.Modified;
				db.SaveChanges();
			}
			List<BudgetItem> budgetItemList = db.BudgetItems.AsNoTracking().Where(h => h.Budget.HouseholdId == user.HouseholdId.Value).OrderBy(c => c.Budget.BudgetName).ThenBy(c => c.ItemName).ToList();
			return (budgetItemList);
		}

		public List<BudgetItem> CreateCustomBudget(string budgetName, string itemType, decimal itemAmount, string itemName)
		{
			Budget budget = new Budget();
			BudgetItem budgetItem = new BudgetItem();
			BudgetItemType budgetItemType = new BudgetItemType();
			var userId = HttpContext.Current.User.Identity.GetUserId();
			var user = db.Users.Find(userId);
			var householdId = user.HouseholdId.Value;
			DateTime now = DateTime.Now;

			if (db.Budgets.Where(h => h.HouseholdId == householdId).ToList().Any(i => i.BudgetName.ToLower().Replace(" ", "") == budgetName.ToLower().Replace(" ", "")))
			{
				budget = db.Budgets.Where(h => h.HouseholdId == householdId).FirstOrDefault(b => b.BudgetName.ToLower() == budgetName.ToLower().Replace(" ", ""));
				budgetItemType = db.BudgetItemTypes.FirstOrDefault(n => n.ItemCategory == budget.BudgetName);
			}
			else
			{
				budget.HouseholdId = householdId;
				budget.BudgetName = budgetName;
				budget.BudgetType = itemType;
				db.Budgets.Add(budget);
				db.SaveChanges();
				budgetItemType.ItemCategory = budgetName;
				budgetItemType.ItemType = itemType;
				db.BudgetItemTypes.Add(budgetItemType);
				db.SaveChanges();
			}
			budgetItem.ItemName = itemName;
			budgetItem.StartDate = now;
			budgetItem.BudgetId = budget.Id;
			budgetItem.ItemAmount = itemAmount;
			budgetItem.BudgetItemTypeId = budgetItemType.Id;
			db.BudgetItems.Add(budgetItem);
			db.SaveChanges();
			budget.BudgetAmount += itemAmount;
			db.Entry(budget).State = EntityState.Modified;
			db.SaveChanges();
			List<BudgetItem> budgetItemList = db.BudgetItems.AsNoTracking().Where(h => h.Budget.HouseholdId == user.HouseholdId.Value).OrderBy(c => c.Budget.BudgetName).ThenBy(c => c.ItemName).ToList();
			return (budgetItemList);
		}

		//public Dictionary<string, string> ListBudgetItemTypes()
		//{
		//	var budgetItemTypes = new Dictionary<string, string>();
		//	foreach (var item in db.BudgetItemTypes)
		//	{
		//		budgetItemTypes.Add(item.ItemCategory, item.ItemType);
		//	}
		//	return budgetItemTypes;
		//}
	}
}