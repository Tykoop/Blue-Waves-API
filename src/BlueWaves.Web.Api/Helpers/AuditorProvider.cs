namespace Esentis.BlueWaves.Web.Api.Helpers
{
	using System;
	using System.Security.Claims;

	using Kritikos.Configuration.Persistence.Services;

	using Microsoft.AspNetCore.Http;

	public class AuditorProvider : IAuditorProvider<Guid>
	{
		private readonly IHttpContextAccessor accessor;

		public AuditorProvider(IHttpContextAccessor accessor) => this.accessor = accessor;

		#region Implementation of IAuditorProvider<out Guid>
		public Guid GetAuditor() => Guid.TryParse(
				accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
				out var guid)
				? guid
				: GetFallbackAuditor();

		/// <inheritdoc />
		public Guid GetFallbackAuditor() => Guid.Empty;
		#endregion
	}
}
