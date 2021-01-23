namespace Esentis.BlueWaves.Web.Api.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Identity.UI.Services;

	public class EmailSender : IEmailSender
	{
		#region Implementation of IEmailSender
		/// <inheritdoc />
		public Task SendEmailAsync(string email, string subject, string htmlMessage) => Task.CompletedTask;
		#endregion
	}
}
