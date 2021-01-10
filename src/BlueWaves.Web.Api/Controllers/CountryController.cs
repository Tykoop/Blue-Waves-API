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

	[Microsoft.AspNetCore.Components.Route("api/country")]
	public class CountryController : BaseController<CountryController>
	{
		public CountryController(ILogger<CountryController> logger, BlueWavesDbContext ctx)
			: base(logger, ctx)
		{
		}

		[HttpPost("")]
		public async Task<ActionResult> AddCountry(AddCountryDto addCountry, CancellationToken token = default)
		{
			var continent = await Context.Continents.SingleOrDefaultAsync(x => x.Id == addCountry.continentId, token);

			if (continent == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Continent));
				return NotFound("Continent not found");
			}

			var country = new Country
			{
				Code = addCountry.Code,
				Continent = continent,
				Currency = addCountry.Currency,
				Description = addCountry.Description,
				Name = addCountry.Name,
			};
			await Context.Countries.AddAsync(country, token);
			await Context.SaveChangesAsync(token);
			return Ok("Country added");
		}

		[HttpDelete("")]
		public async Task<ActionResult> DeleteCountry(long id, CancellationToken token)
		{
			var country = await Context.Countries.SingleOrDefaultAsync(x => x.Id == id, token);
			if (country == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Country));
				return NotFound("Country not found");
			}

			Context.Countries.Remove(country);
			await Context.SaveChangesAsync(token);
			return Ok("Country deleted");
		}
	}
}
