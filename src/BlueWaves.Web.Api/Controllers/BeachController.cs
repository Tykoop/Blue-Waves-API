namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using NetTopologySuite.Geometries;


	[Route("api/beach")]
	public class BeachController : BaseController<BeachController>
	{

		public BeachController(ILogger<BeachController> logger, BlueWavesDbContext ctx)
			: base(logger, ctx)
		{
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Beach>> GetBeach(long id, CancellationToken token = default)
		{
			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: token);
			var result = new BeachDto(Id: beach.Id, Name: beach.Name, Latitude: beach.Coordinates.X,
				Longtitude: beach.Coordinates.Y, Description: beach.Description);
			return beach == null
				? NotFound()
				: Ok(result);
		}

		[HttpPost("add")]
		public async Task<ActionResult<BeachDto>> AddBeach(AddBeachDto addBeachDto)
		{
			var beach = new Beach { Coordinates = new Point(addBeachDto.Latitude, addBeachDto.Longtitude), Name = addBeachDto.Name, Description = addBeachDto.Description };
			Context.Beaches.Add(beach);
			await Context.SaveChangesAsync();
			return Ok(beach.toDto());
		}
	}
}
