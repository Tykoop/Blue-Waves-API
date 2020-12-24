namespace Esentis.BlueWaves.Web.Api.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;

	[ApiController]
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
	}
}
