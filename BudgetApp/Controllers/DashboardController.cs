using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BudgetApp.Models;
using System.Data.Entity;
using BudgetApp.Helpers;
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace BudgetApp.Controllers
{
	public class DashboardController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		private UserHelper userHelper = new UserHelper();
		private HouseholdHelper householdHelper = new HouseholdHelper();

		// GET: Dashboard
		public ActionResult DashboardGuestAtFrontDoor()
		{
			DashboardGuestAtFrontDoorViewModel guestAtFrontDoorVM = new DashboardGuestAtFrontDoorViewModel();
			guestAtFrontDoorVM.ReturnUrl = Request.Url.PathAndQuery;
			return View(guestAtFrontDoorVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DashboardGuestAtFrontDoor([Bind(Include = "householdName, accountName, accountDescription, createdAmount")] DashboardGuestAtFrontDoorViewModel GuestAtFrontDoorVM)
		{
			if (ModelState.IsValid)
			{
				GuestAtFrontDoorVM.ReturnUrl = Request.Url.PathAndQuery;
				Household household = new Household();
				BankAccount bankAccount = new BankAccount();
				Budget budget = new Budget();
				var user = db.Users.Find(User.Identity.GetUserId());
				if (user.HouseholdId == null)
				{
					household.HouseholdName = GuestAtFrontDoorVM.householdName;
					household.Active = true;
					household.DateCreated = DateTime.Now;
					userHelper.RemoveUserFromRole(user.Id, "GuestAtFrontDoor");
					userHelper.AddUserToRole(user.Id, "HeadOfHousehold");
					user.HouseholdId = household.Id;
					household.OwnerId = user.Id;
					household.Members.Add(user);
					db.Households.Add(household);
					db.SaveChanges();
					bankAccount.AccountName = GuestAtFrontDoorVM.accountName;
					bankAccount.AccountDescription = GuestAtFrontDoorVM.accountDescription;
					bankAccount.CreatedAmount = GuestAtFrontDoorVM.createdAmount;
					bankAccount.CreatedDate = DateTime.Now;
					bankAccount.Balance = GuestAtFrontDoorVM.createdAmount;
					bankAccount.HouseholdId = household.Id;
					db.BankAccounts.Add(bankAccount);
					db.SaveChanges();
					return View();
				}
				else
				{
					return View();
				}
			}
			return View();
		}


		// GET: Dashboard
		public ActionResult DashboardHeadOfHousehold()
		{
			DashboardHeadOfHouseholdViewModel headOfHouseholdVM = new DashboardHeadOfHouseholdViewModel();
			headOfHouseholdVM.ReturnUrl = Request.Url.PathAndQuery;
			var user = db.Users.Find(User.Identity.GetUserId());
			var householdId = user.HouseholdId.Value;
			headOfHouseholdVM.membersList = householdHelper.ListHouseholdMembers(householdId);
			headOfHouseholdVM.budgetList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).OrderBy(c => c.BudgetName).ToList();
			headOfHouseholdVM.budgetItemList = db.BudgetItems.Where(h => h.Budget.HouseholdId == householdId).ToList();
			headOfHouseholdVM.bankAccountList = db.BankAccounts.Where(h => h.HouseholdId == householdId).ToList();
			headOfHouseholdVM.transactionList = db.Transactions.Where(t => t.TransactionIsDeleted == false).Where(h => h.BankAccount.HouseholdId == householdId).OrderByDescending(d => d.DateCreated).ToList();
			ViewBag.BankAccountId = new SelectList(headOfHouseholdVM.bankAccountList.OrderBy(n => n.AccountName), "Id", "AccountName");
			ViewBag.BudgetItemId = householdHelper.CreateBudgetItemsSelectList();
			//headOfHouseholdVM.membersList = db.Users.Where(i => i.HouseholdId == user.HouseholdId.Value).ToList();
			return View(headOfHouseholdVM);
		}

		// GET: Dashboard
		public ActionResult DashboardMemberOfHousehold()
		{
			DashboardMemberOfHouseholdViewModel memberOfHouseholdVM = new DashboardMemberOfHouseholdViewModel();
			memberOfHouseholdVM.ReturnUrl = Request.Url.PathAndQuery;
			var user = db.Users.Find(User.Identity.GetUserId());
			var householdId = user.HouseholdId.Value;
			memberOfHouseholdVM.budgetList = db.Budgets.Where(h => h.HouseholdId == householdId).Include(i => i.BudgetItems).OrderBy(c => c.BudgetName).ToList();
			memberOfHouseholdVM.budgetItemList = db.BudgetItems.Where(h => h.Budget.HouseholdId == householdId).ToList();
			memberOfHouseholdVM.bankAccountList = db.BankAccounts.Where(h => h.HouseholdId == householdId).ToList();
			memberOfHouseholdVM.transactionList = db.Transactions.Where(t => t.TransactionIsDeleted == false).Where(h => h.BankAccount.HouseholdId == householdId).OrderByDescending(d => d.DateCreated).ToList();
			ViewBag.BankAccountId = new SelectList(memberOfHouseholdVM.bankAccountList.OrderBy(n => n.AccountName), "Id", "AccountName");
			ViewBag.BudgetItemId = householdHelper.CreateBudgetItemsSelectList();
			memberOfHouseholdVM.membersList = db.Users.Where(i => i.HouseholdId == user.HouseholdId.Value).ToList();
			return View(memberOfHouseholdVM);
		}


		// POST: BudgetItems/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include = "Id,Name,Description,BudgetId,BudgetItemTypeId")] BudgetItem budgetItem)
		public ActionResult BudgetCreate([Bind(Include = "BudgetName,ItemAmount")] BudgetItem budgetItem, Budget budget, BudgetItemType budgetItemType)
		{
			if (ModelState.IsValid)
			{
				var addedBudgets = householdHelper.CreateNewBudget(budget.BudgetName, budgetItem.ItemAmount);
				ViewBag.BudgetItemId = householdHelper.CreateBudgetItemsSelectList();
				return PartialView("AddedBudgets", addedBudgets);
			}
			return View();
		}


		// POST: BudgetItems/CreateCustom
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateCustom([Bind(Include = "ItemType,ItemCategory,ItemAmount,ItemName")] BudgetItem budgetItem, Budget budget, BudgetItemType budgetItemType)
		{
			if (ModelState.IsValid && budgetItem.ItemName != null && budgetItemType.ItemCategory != null && budgetItem.ItemAmount != 0)
			{
				if (budgetItemType.ItemType == "Select")
				{
					budgetItemType.ItemType = "Expense";
				}
				var addedBudgets = householdHelper.CreateCustomBudget(budgetItemType.ItemCategory, budgetItemType.ItemType, budgetItem.ItemAmount, budgetItem.ItemName);
				return PartialView("AddedBudgets", addedBudgets);
			}
			return View();
		}


		// POST: BankAccounts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult BankAccountCreate([Bind(Include = "AccountName,AccountDescription,CreatedAmount")] BankAccount bankAccount)
		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				bankAccount.HouseholdId = user.HouseholdId.Value;
				bankAccount.CreatedDate = DateTime.Now;
				bankAccount.Balance = bankAccount.CreatedAmount;
				db.BankAccounts.Add(bankAccount);
				db.SaveChanges();
				var addedBankAccounts = db.BankAccounts.AsNoTracking().Where(h => h.HouseholdId == user.HouseholdId.Value).ToList();
				return PartialView("AddedBankAccounts", addedBankAccounts);
			}
			return View();
		}

		[HttpGet]
		public ActionResult BudgetStatusUpdate()
		{
			BudgetStatusViewModel budgetStatusVM = new BudgetStatusViewModel();
			var user = db.Users.Find(User.Identity.GetUserId());
			budgetStatusVM.budgetItemIncomeList = db.BudgetItems.Where(h => h.Budget.HouseholdId == user.HouseholdId.Value).Where(i => i.BudgetItemType.ItemType == "Income").OrderBy(i => i.Budget.BudgetName).ThenBy(i => i.ItemName).ToList();
			budgetStatusVM.budgetItemExpenseList = db.BudgetItems.Where(h => h.Budget.HouseholdId == user.HouseholdId.Value).Where(i => i.BudgetItemType.ItemType == "Expense").OrderBy(i => i.Budget.BudgetName).ThenBy(i => i.ItemName).ToList();
			budgetStatusVM.budgetList = db.Budgets.Where(h => h.HouseholdId == user.HouseholdId.Value).OrderBy(i => i.BudgetName).ToList();
			budgetStatusVM.budgetStatusData = new List<BudgetStatusData>();
			budgetStatusVM.budgetItemExpenseStatusData = new List<BudgetItemStatusData>();
			budgetStatusVM.budgetItemIncomeStatusData = new List<BudgetItemStatusData>();
			budgetStatusVM.budgetIncomeTotal = budgetStatusVM.budgetItemIncomeList.Sum(i => i.ItemAmount);
			budgetStatusVM.budgetExpenseTotal = budgetStatusVM.budgetItemExpenseList.Sum(i => i.ItemAmount);
			var budgetTotal = budgetStatusVM.budgetList.Sum(i => i.BudgetAmount) - budgetStatusVM.budgetIncomeTotal;
			foreach (var item in budgetStatusVM.budgetList)
			{
				if (item.BudgetType == "Income")
				{
					budgetStatusVM.budgetStatusData.Add(new BudgetStatusData { budget = item, budgetPercentageSpent = (item.BudgetCurrentAmount / item.BudgetAmount).ToString("0.##%"), budgetPercentSpent = (item.BudgetCurrentAmount / item.BudgetAmount * 100).ToString(), budgetItemsCount = item.BudgetItems.Count(), percentTotalBudget = (item.BudgetAmount / budgetStatusVM.budgetIncomeTotal).ToString("0.##%") });
				}
				else
				{
					budgetStatusVM.budgetStatusData.Add(new BudgetStatusData { budget = item, budgetPercentageSpent = (item.BudgetCurrentAmount / item.BudgetAmount).ToString("0.##%"), budgetPercentSpent = (item.BudgetCurrentAmount / item.BudgetAmount * 100).ToString(), budgetItemsCount = item.BudgetItems.Count(), percentTotalBudget = (item.BudgetAmount / budgetStatusVM.budgetExpenseTotal).ToString("0.##%") });
				}
			}
			foreach (var item in budgetStatusVM.budgetItemIncomeList)
			{
				budgetStatusVM.budgetItemIncomeStatusData.Add(new BudgetItemStatusData { budgetItem = item, budgetItemPercentageSpent = item.ItemCurrentAmount / item.ItemAmount * 100 });
			}
			foreach (var item in budgetStatusVM.budgetItemExpenseList)
			{
				budgetStatusVM.budgetItemExpenseStatusData.Add(new BudgetItemStatusData { budgetItem = item, budgetItemPercentageSpent = item.ItemCurrentAmount / item.ItemAmount * 100 });
			}
			return PartialView("BudgetStatus", budgetStatusVM);
		}

		[HttpPost]
		public ActionResult TransactionsListUpdate()
		{
			var user = db.Users.Find(User.Identity.GetUserId());
			var addedTransactions = db.Transactions.AsNoTracking().Where(t => t.TransactionIsDeleted == false).Where(i => i.BudgetItem.Budget.HouseholdId == user.HouseholdId.Value).Include(i => i.BankAccount).Include(i => i.BudgetItem).OrderByDescending(i => i.DateCreated).ThenBy(i => i.BudgetItem.ItemName).ToList();
			return PartialView("AddedTransactions", addedTransactions);
		}

		[HttpPost]
		public ActionResult BankAccountsListUpdate()
		{
			var user = db.Users.Find(User.Identity.GetUserId());
			var addedBankAccounts = db.BankAccounts.AsNoTracking().Where(h => h.HouseholdId == user.HouseholdId.Value).ToList();
			return PartialView("AddedBankAccounts", addedBankAccounts);
		}

		[HttpPost]
		public ActionResult BudgetItemsListUpdate()
		{
			var user = db.Users.Find(User.Identity.GetUserId());
			var budgetItemList = db.BudgetItems.AsNoTracking().Where(h => h.Budget.HouseholdId == user.HouseholdId.Value).OrderBy(i => i.Budget.BudgetName).ThenBy(i => i.ItemName).ToList();
			return PartialView("AddedBudgets", budgetItemList);
		}

		[HttpPost]
		public ActionResult CreateAddTransactionForm()
		{
			AddTransactionViewModel addTransactionVM = new AddTransactionViewModel();
			var user = db.Users.Find(User.Identity.GetUserId());
			var householdId = user.HouseholdId.Value;
			ViewBag.BankAccountId = householdHelper.CreateBankAccountsSelectList();
			ViewBag.BudgetItemId = householdHelper.CreateBudgetItemsSelectList();
			return PartialView("AddTransactionModalForm", addTransactionVM);
		}

		// POST: Transaction/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TransactionCreate([Bind(Include = "Description,TransactionAmount,Reconciled,ReconciledAmount,BankAccountId")] Transaction transaction, string BudgetItemId)
		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				var householdId = user.HouseholdId.Value;
				var budgetItemId = Convert.ToInt16(BudgetItemId);
				transaction.BudgetItemId = budgetItemId;
				transaction.DateCreated = DateTime.Now;
				transaction.EnteredById = user.Id;
				db.Transactions.Add(transaction);
				db.SaveChanges();
				BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(i => i.Id == transaction.BudgetItemId);
				Budget budget = db.Budgets.FirstOrDefault(i => i.Id == budgetItem.BudgetId);
				BankAccount bankAccount = db.BankAccounts.FirstOrDefault(i => i.Id == transaction.BankAccountId);
				if (budget.BudgetType == "Income")
				{
					bankAccount.Balance += transaction.TransactionAmount;
				}
				else
				{
					bankAccount.Balance -= transaction.TransactionAmount;
				}
				budgetItem.ItemCurrentAmount += transaction.TransactionAmount;
				budget.BudgetCurrentAmount += transaction.TransactionAmount;
				db.Entry(budgetItem).State = EntityState.Modified;
				db.Entry(budget).State = EntityState.Modified;
				db.Entry(bankAccount).State = EntityState.Modified;
				db.SaveChanges();
				var addedTransactions = db.Transactions.AsNoTracking().Where(t => t.TransactionIsDeleted == false).Where(i => i.BudgetItem.Budget.HouseholdId == householdId).Include(i => i.BankAccount).Include(i => i.BudgetItem).OrderByDescending(i => i.DateCreated).ToList();
				return PartialView("AddedTransactions", addedTransactions);
			}
			return View();
		}

		// POST: Transactions/Delete/5
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public ActionResult TransactionDelete(int id)
		{
			var user = db.Users.Find(User.Identity.GetUserId());
			var householdId = user.HouseholdId.Value;
			Transaction transaction = db.Transactions.Find(id);
			transaction.TransactionIsDeleted = true;
			db.Entry(transaction).State = EntityState.Modified;
			db.SaveChanges();
			BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(i => i.Id == transaction.BudgetItemId);
			Budget budget = db.Budgets.FirstOrDefault(i => i.Id == budgetItem.BudgetId);
			BankAccount bankAccount = db.BankAccounts.FirstOrDefault(i => i.HouseholdId == householdId);
			if (budget.BudgetType == "Income")
			{
				bankAccount.Balance -= transaction.TransactionAmount;
			}
			else
			{
				bankAccount.Balance += transaction.TransactionAmount;
			}
			budgetItem.ItemCurrentAmount -= transaction.TransactionAmount;
			budget.BudgetCurrentAmount -= transaction.TransactionAmount;
			db.Entry(budgetItem).State = EntityState.Modified;
			db.SaveChanges();
			db.Entry(budget).State = EntityState.Modified;
			db.SaveChanges();
			db.Entry(bankAccount).State = EntityState.Modified;
			db.SaveChanges();
			var addedTransactions = db.Transactions.Where(t => t.TransactionIsDeleted == false).Where(i => i.BudgetItem.Budget.HouseholdId == householdId).Include(i => i.BankAccount).Include(i => i.BudgetItem).OrderByDescending(i => i.DateCreated).ToList();
			return PartialView("AddedTransactions", addedTransactions);
		}

		public ActionResult ConfirmTransactionDelete(int id)
		{
			//Transaction transaction = db.Transactions.Find(id);
			var transactionToDelete = db.Transactions.Where(i => i.Id == id).ToList();
			return PartialView("TransactionToDelete", transactionToDelete);
		}

		public ActionResult CreateEditTransactionForm(int id)
		{
			AddTransactionViewModel editTransactionVM = new AddTransactionViewModel();
			editTransactionVM.transaction = db.Transactions.Find(id);
			var user = db.Users.Find(User.Identity.GetUserId());
			var householdId = user.HouseholdId.Value;
			var bankAccountList = db.BankAccounts.Where(h => h.HouseholdId == householdId).OrderBy(n => n.AccountName).ToList();
			SelectList currentBankAccounts = new SelectList(bankAccountList, "Id", "AccountName", editTransactionVM.transaction.BankAccountId);
			ViewBag.BankAccountId = currentBankAccounts;
			var budgetItems = db.BudgetItems.Where(i => i.Budget.HouseholdId == householdId).Include(i => i.BudgetItemType).OrderBy(i => i.BudgetItemType.ItemCategory).ThenBy(i => i.ItemName).ToList();
			SelectList currentBudgetItems = new SelectList(budgetItems, "Id", "ItemName", "BudgetItemType.ItemCategory", editTransactionVM.transaction.BudgetItemId, null);
			ViewBag.BudgetItemId = currentBudgetItems;
			return PartialView("EditTransactionModalForm", editTransactionVM);
		}

		// POST: Transaction/Edit
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TransactionEdit([Bind(Include = "Id,Description,TransactionAmount,Reconciled,ReconciledAmount,BankAccountId,BudgetItemId")] Transaction transaction)
		{
			if (ModelState.IsValid)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				var householdId = user.HouseholdId.Value;
				var budgetItemId = transaction.BudgetItemId;
				BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == budgetItemId);
				Budget budget = db.Budgets.FirstOrDefault(b => b.Id == budgetItem.BudgetId);
				BudgetItemType budgetItemType = db.BudgetItemTypes.FirstOrDefault(b => b.Id == budgetItem.BudgetItemTypeId);
				BankAccount bankAccount = db.BankAccounts.FirstOrDefault(i => i.Id == transaction.BankAccountId);
				Transaction origTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
				BudgetItem origBudgetItem = db.BudgetItems.FirstOrDefault(i => i.Id == origTransaction.BudgetItemId);
				Budget origBudget = db.Budgets.FirstOrDefault(i => i.Id == origTransaction.BudgetItem.BudgetId);
				BankAccount origBankAccount = db.BankAccounts.FirstOrDefault(i => i.Id == origTransaction.BankAccountId);
				if (transaction.ReconciledAmount > 0)
				{
					transaction.Reconciled = true;
					if (origBudget.BudgetType == "Income")
					{
						origBankAccount.Balance -= transaction.TransactionAmount;
						origBankAccount.Balance += transaction.ReconciledAmount;
					}
					else
					{
						origBankAccount.Balance += transaction.TransactionAmount;
						origBankAccount.Balance -= transaction.ReconciledAmount;
					}
					origBudgetItem.ItemCurrentAmount -= transaction.TransactionAmount;
					origBudget.BudgetCurrentAmount -= transaction.TransactionAmount;
					origBudgetItem.ItemCurrentAmount += transaction.ReconciledAmount;
					origBudget.BudgetCurrentAmount += transaction.ReconciledAmount;
					db.Entry(origBudgetItem).State = EntityState.Modified;
					db.SaveChanges();
					db.Entry(origBudget).State = EntityState.Modified;
					db.SaveChanges();
					db.Entry(origBankAccount).State = EntityState.Modified;
					db.SaveChanges();
					transaction.TransactionAmount = transaction.ReconciledAmount;
					transaction.ReconciledAmount = 0;
				}
				if (transaction.BankAccountId != origTransaction.BankAccountId)
				{
					//BankAccount bankAccount = db.BankAccounts.FirstOrDefault(i => i.Id == transaction.BankAccountId);
					if (origBudget.BudgetType == "Income")
					{
						origBankAccount.Balance -= transaction.TransactionAmount;
						bankAccount.Balance += transaction.TransactionAmount;
					}
					else
					{
						origBankAccount.Balance += transaction.TransactionAmount;
						bankAccount.Balance -= transaction.TransactionAmount;
					}
					db.Entry(origBankAccount).State = EntityState.Modified;
					db.SaveChanges();
					db.Entry(bankAccount).State = EntityState.Modified;
					db.SaveChanges();
				}
				if (transaction.BudgetItemId != origTransaction.BudgetItemId)
				{
					origBudgetItem.ItemCurrentAmount -= transaction.TransactionAmount;
					db.Entry(origBudgetItem).State = EntityState.Modified;
					db.SaveChanges();
					//BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(i => i.Id == transaction.BudgetItemId);
					budgetItem.ItemCurrentAmount += transaction.TransactionAmount;
					db.Entry(budgetItem).State = EntityState.Modified;
					db.SaveChanges();
					if (budgetItem.BudgetId != origTransaction.BudgetItem.BudgetId)
					{
						origBudget.BudgetCurrentAmount -= transaction.TransactionAmount;
						db.Entry(origBudget).State = EntityState.Modified;
						db.SaveChanges();
						//Budget budget = db.Budgets.FirstOrDefault(i => i.Id == transaction.BudgetItem.BudgetId);
						budget.BudgetCurrentAmount += transaction.TransactionAmount;
						db.Entry(budget).State = EntityState.Modified;
						db.SaveChanges();
						if (budget.BudgetType != origBudget.BudgetType)
						{
							//BankAccount bankAccount = db.BankAccounts.FirstOrDefault(i => i.Id == transaction.BankAccountId);
							if (budget.BudgetType == "Income")
							{
								bankAccount.Balance += transaction.TransactionAmount * 2;
							}
							else
							{
								bankAccount.Balance -= transaction.TransactionAmount * 2;
							}
						}
					}
				}
				transaction.DateCreated = DateTime.Now;
				transaction.EnteredById = user.Id;
				db.Entry(transaction).State = EntityState.Modified;
				db.SaveChanges();
				var addedTransactions = db.Transactions.AsNoTracking().Where(t => t.TransactionIsDeleted == false).Where(i => i.BudgetItem.Budget.HouseholdId == householdId).Include(i => i.BankAccount).Include(i => i.BudgetItem).OrderByDescending(i => i.DateCreated).ToList();
				return PartialView("AddedTransactions", addedTransactions);
			}
			return View();
		}
	}
}
