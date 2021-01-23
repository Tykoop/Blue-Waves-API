namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Web.Api.Helpers;

	using Microsoft.AspNetCore.Connections;
	using Microsoft.Extensions.Logging;

	public class AdminController : BaseController<AdminController>
	{
		/// <inheritdoc />
		public AdminController(ILogger<AdminController> logger, BlueWavesDbContext ctx)
			: base(logger, ctx)
		{
		}
	}
}
