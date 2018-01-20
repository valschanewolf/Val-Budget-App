using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }
		public decimal TransactionAmount { get; set; }
		public bool Reconciled { get; set; }
		public decimal ReconciledAmount { get; set; }
		public bool TransactionIsDeleted { get; set; }

		public int BankAccountId { get; set; }
		public int? BudgetItemId { get; set; }
		public string EnteredById { get; set; }

		public virtual BankAccount BankAccount { get; set; }
		public virtual BudgetItem BudgetItem { get; set; }
		public virtual ApplicationUser EnteredBy { get; set; }

	}
}