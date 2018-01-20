using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class Budget
	{
		public int Id { get; set; }
		public string BudgetName { get; set; }
		public decimal BudgetAmount { get; set; }
		public decimal BudgetCurrentAmount { get; set; }
		public bool BudgetIsDeleted { get; set; }
		public string BudgetType { get; set; }

		public int HouseholdId { get; set; }
		//public int CategoryId { get; set; }

		public virtual Household Household { get; set; }
		//public virtual BudgetCategory BudgetCategory { get; set; }

		public virtual ICollection<BudgetItem>  BudgetItems { get; set; }

		public Budget()
		{
			this.BudgetItems = new HashSet<BudgetItem>();
		}
	}
}