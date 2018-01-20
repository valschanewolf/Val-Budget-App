using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
	public class Notification
	{
		public int Id { get; set; }
		public DateTime DateSent { get; set; }
		public string NotificationReason { get; set; }
		public string NotificationContent { get; set; }
		public bool Archived { get; set; }

		public string SenderId { get; set; }
		public string RecipientId { get; set; }

		public virtual ApplicationUser Sender { get; set; }
		public virtual ApplicationUser Recipient { get; set; }
	}
}