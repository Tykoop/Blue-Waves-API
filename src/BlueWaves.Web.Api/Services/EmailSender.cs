namespace Esentis.BlueWaves.Web.Api.Services
{
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
