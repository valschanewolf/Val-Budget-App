using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class BankAccount
	{
		public int Id { get; set; }

		public string AccountName { get; set; }
		public string AccountDescription { get; set; }
		public decimal Balance { get; set; }
		//[Required]
		//[DataType(DataType.Currency)]
		//[RegularExpression("[0-9]+(.[0-9][0-9]?)?", ErrorMessage = "Amount must be numeric and at least $10.00")]
		public decimal CreatedAmount { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool AccountIsDeleted { get; set; }

		public int HouseholdId { get; set; }

		public virtual Household Household { get; set; }
	}
}