using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class BudgetItemType
	{
		public int Id { get; set; }
		public string ItemType { get; set; }
		public string ItemCategory { get; set; }

		public virtual ICollection<BudgetItem> BudgetItems { get; set; }

		public BudgetItemType()
		{
			this.BudgetItems = new HashSet<BudgetItem>();
		}
	}
}