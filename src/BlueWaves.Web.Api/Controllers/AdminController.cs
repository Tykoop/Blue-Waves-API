namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Web.Api.Helpers;

	using Kritikos.PureMap.Contracts;

	using Microsoft.Extensions.Logging;

	public class AdminController : BaseController<AdminController>
	{
		public AdminController(ILogger<AdminController> logger, BlueWavesDbContext ctx, IPureMapper mapper)
			: base(logger, ctx, mapper)
		{
		}
	}
}
