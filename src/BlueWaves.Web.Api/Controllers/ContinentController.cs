namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[Route("api/continent")]
	public class ContinentController : BaseController<ContinentController>
	{
		public ContinentController(ILogger<ContinentController> logger, BlueWavesDbContext ctx)
			: base(logger, ctx)
		{
		}

		[HttpPost("")]
		public async Task<ActionResult> AddContinent(AddContinentDto addContinentDto, CancellationToken token = default)
		{
			var continent = new Continent { Name = addContinentDto.Name, Size = addContinentDto.Size };
			await Context.Continents.AddAsync(continent, token);
			await Context.SaveChangesAsync(token);
			return Ok("Continent added");
		}

		[HttpDelete("")]
		public async Task<ActionResult> DeleteContinent(long id, CancellationToken token)
		{
			var continent = await Context.Continents.SingleOrDefaultAsync(x => x.Id == id, token);
			if (continent == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Continent));
				return NotFound("Continent not found");
			}

			Context.Continents.Remove(continent);
			await Context.SaveChangesAsync(token);
			Logger.LogInformation(BWLogTemplates.Deleted, nameof(Continent), id);
			return Ok("Continent deleted");
		}
	}
}
