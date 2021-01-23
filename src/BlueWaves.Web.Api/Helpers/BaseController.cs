namespace Esentis.BlueWaves.Web.Api.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;

	[ApiController]
	[Authorize]
	public abstract class BaseController<T> : ControllerBase
		where T : BaseController<T>
	{

		protected BaseController(ILogger<T> logger, BlueWavesDbContext ctx)
		{
			Logger = logger;
			Context = ctx;
		}

		protected ILogger<T> Logger { get; init; }

		protected BlueWavesDbContext Context { get; init; }
		// HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty

		protected Guid RetreiveUserId() =>
			Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var guid)
				? guid
				: Guid.Empty;

	}
}
