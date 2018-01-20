using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace BudgetApp.Helpers
{
	public class EmailHelper
	{
			public static async Task SendInvite(string email, string code)
			{
				//Get data from our private.Config for usage within the SMTP object
				var gmailUsername = WebConfigurationManager.AppSettings["username"];
				var gmailPassword = WebConfigurationManager.AppSettings["password"];
				var gmailHost = WebConfigurationManager.AppSettings["host"];
				var gmailPort = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);
				var emailFrom = WebConfigurationManager.AppSettings["emailfrom"];
			using (var smtp = new SmtpClient()
				{
					Host = gmailHost,
					Port = gmailPort,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(gmailUsername, gmailPassword)
				})

					try
					{
						await smtp.SendMailAsync(emailFrom, email, "Join a household", "Please join a household by clicking <a href=\"" + code + "\">here</a>");
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
						await Task.FromResult(0);
					}
			}
		}
	}