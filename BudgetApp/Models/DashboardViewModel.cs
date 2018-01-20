using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetApp.Models
{

	public class DashboardGuestAtFrontDoorViewModel
	{
		public string ReturnUrl { get; set; }

		[Required(ErrorMessage = "You are required to give your household a name.")]
		[Display(Name = "Household Name")]
		public string householdName { get; set; }

		[Required(ErrorMessage = "You are required to provide a name for the account.")]
		[Display(Name = "Account Name")]
		public string accountName { get; set; }

		[Required(ErrorMessage = "Please provide a description for the account.")]
		[Display(Name = "Account Description")]
		//[DataType(DataType.MultilineText)]
		public string accountDescription { get; set; }

		[Required(ErrorMessage = "The starting account balance must be at least $1.00")]
		[DataType(DataType.Currency)]
		[Range(1, 9999999, ErrorMessage = "The starting account balance must be at least $1.00")]
		public decimal createdAmount { get; set; }
	}

	public class DashboardHeadOfHouseholdViewModel
	{
		public string ReturnUrl { get; set; }
		public Household household { get; set; }
		public BankAccount bankAccount { get; set; }
		public List<BankAccount> bankAccountList { get; set; }
		public Budget budget { get; set; }
		public BudgetItem budgetItem { get; set; }
		public List<Budget> budgetList { get; set; }
		public List<BudgetItem> budgetItemList { get; set; }
		public BudgetItemType budgetItemType { get; set; }
		public Transaction transaction { get; set; }
		public List<Transaction> transactionList { get; set; }
		public Invitation invitation { get; set; }
		public List<Invitation> invitationList { get; set; }
		public List<ApplicationUser> membersList { get; set; }
	}

	public class AddTransactionViewModel
	{
		public Household household { get; set; }
		public BudgetItem budgetItem { get; set; }
		public List<BudgetItem> budgetItemList { get; set; }
		public BankAccount bankAccount { get; set; }
		public List<BankAccount> bankAccountList { get; set; }
		public Transaction transaction { get; set; }
	}

	public class BudgetStatusData
	{
		public Budget budget { get; set; }
		public string budgetPercentageSpent { get; set; }
		public string budgetPercentSpent { get; set; }
		public int budgetItemsCount { get; set; }
		public string percentTotalBudget { get; set; }
	}

	public class BudgetItemStatusData
	{
		public BudgetItem budgetItem { get; set; }
		public decimal budgetItemPercentageSpent { get; set; }
	}

	public class BudgetStatusViewModel
	{
		public List<BudgetStatusData> budgetStatusData { get; set; }
		public List<BudgetItemStatusData> budgetItemExpenseStatusData { get; set; }
		public List<BudgetItemStatusData> budgetItemIncomeStatusData { get; set; }
		public List<BudgetItem> budgetItemIncomeList { get; set; }
		public List<BudgetItem> budgetItemExpenseList { get; set; }
		public List<Budget> budgetList { get; set; }
		public decimal currentBudgetIncomeTotal { get; set; }
		public decimal currentBudgetExpenseTotal { get; set; }
		public decimal budgetExpenseTotal { get; set; }
		public decimal budgetIncomeTotal { get; set; }
	}

	public class DashboardMemberOfHouseholdViewModel
	{
		public string ReturnUrl { get; set; }
		public Household household { get; set; }
		public BankAccount bankAccount { get; set; }
		public List<BankAccount> bankAccountList { get; set; }
		public Budget budget { get; set; }
		public BudgetItem budgetItem { get; set; }
		public List<Budget> budgetList { get; set; }
		public List<BudgetItem> budgetItemList { get; set; }
		public BudgetItemType budgetItemType { get; set; }
		public Transaction transaction { get; set; }
		public List<Transaction> transactionList { get; set; }
		public Invitation invitation { get; set; }
		public List<Invitation> invitationList { get; set; }
		public List<ApplicationUser> membersList { get; set; }
	}
}