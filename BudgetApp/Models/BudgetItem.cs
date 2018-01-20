using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class BudgetItem
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "A budget amount must be at least $1.00")]
		[DataType(DataType.Currency)]
		[Range(0, 9999999, ErrorMessage = "A budget amount must be at least $1.00")]
		public decimal ItemAmount { get; set; }
		public string ItemName { get; set; }
		public decimal ItemCurrentAmount { get; set; }
		public decimal WarningAmount { get; set; }
		public int Frequency { get; set; }
		public bool RetainLeftoverAmount { get; set; }
		public decimal PreviousLeftoverAmount { get; set; }
		public DateTime StartDate { get; set; }
		public bool ItemIsDeleted { get; set; }


		public int BudgetId { get; set; }
		//public int BudgetCatecoryId { get; set; }
		public int BudgetItemTypeId { get; set; }

		public virtual Budget Budget { get; set; }
		//public virtual BudgetCategory BudgetCategory { get; set; }
		public virtual BudgetItemType BudgetItemType { get; set; }
	}
}