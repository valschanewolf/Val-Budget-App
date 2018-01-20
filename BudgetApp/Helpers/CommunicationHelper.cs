using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApp.Helpers
{
	public class CommunicationHelper
	{
		public string GenerateInvitationCode()
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[8];
			var random = new Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			var invitationCode = new string(stringChars);
			return invitationCode;
		}
	}

}