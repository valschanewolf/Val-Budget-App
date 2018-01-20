using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class Household
	{
		public int Id { get; set; }
		public string HouseholdName { get; set; }
		public bool Active { get; set; }
		public DateTime DateCreated { get; set; }
		public string OwnerId { get; set; }

		public virtual ApplicationUser Owner { get; set; }
		public virtual ICollection<ApplicationUser> Members { get; set; }
		public virtual ICollection<Budget> Budgets { get; set; }
		public virtual ICollection<BankAccount> BankAccounts { get; set; }
		public virtual ICollection<Invitation> Invitations { get; set; }
		public virtual ICollection<Notification> Notifications { get; set; }

		public Household()
		{
			this.Members = new HashSet<ApplicationUser>();
			this.Budgets = new HashSet<Budget>();
			this.BankAccounts = new HashSet<BankAccount>();
			this.Invitations = new HashSet<Invitation>();
			this.Notifications = new HashSet<Notification>();
		}
	}
}