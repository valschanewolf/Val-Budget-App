using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class Invitation
	{
		public int Id { get; set; }
		public string InvitationCode { get; set; }
		public string Email { get; set; }
		public DateTime DateSent { get; set; }
		public bool Accepted { get; set; }

		public int HouseholdId { get; set; }

		public virtual Household Household { get; set; }
	}
}