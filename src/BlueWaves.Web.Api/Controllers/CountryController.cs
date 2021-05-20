namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;
	using Esentis.BlueWaves.Web.Models.Dto;
	using Esentis.Ieemdb.Web.Models.SearchCriteria;

	using Kritikos.Extensions.Linq;
	using Kritikos.PureMap;
	using Kritikos.PureMap.Contracts;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[Route("api/country")]
	public class CountryController : BaseController<CountryController>
	{
		public CountryController(ILogger<CountryController> logger, BlueWavesDbContext ctx, IPureMapper mapper)
			: base(logger, ctx, mapper)
		{
		}

		/// <summary>
		/// Returns all Countries.
		/// </summary>
		/// <param name="criteria">Paging criteria.</param>
		/// <response code="200">Gets all countries.</response>
		/// <returns>List of <see cref="CountryDto"/>.</returns>
		[HttpPost("all")]
		public async Task<ActionResult<PagedResult<CountryDto>>> GetAllCountries(PaginationCriteria criteria,
			CancellationToken token = default)
		{
			var countries = Context.Countries.OrderBy(x => x.Id);

			var count = await countries.CountAsync(token);

			var slice = await countries.Slice(criteria.Page, criteria.ItemsPerPage)
				.Project<Country, CountryDto>(Mapper)
				.ToListAsync(token);

			var result = new PagedResult<CountryDto>
			{
				Results = slice,
				Page = criteria.Page,
				TotalElements = count,
				TotalPages = (count / criteria.ItemsPerPage) + 1,
			};

			return Ok(result);
		}

		/// <summary>
		/// Add new Country.
		/// </summary>
		/// <param name="addCountry">Country information.</param>
		/// <response code="201">Country added.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">Continent not found.</response>
		/// <returns>Created <see cref="CountryDto"/>.</returns>
		[HttpPost("add")]
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
				Iso = addCountry.Code,
				Continent = continent,
				Currency = addCountry.Currency,
				Description = addCountry.Description,
				Name = addCountry.Name,
			};
			await Context.Countries.AddAsync(country, token);
			await Context.SaveChangesAsync(token);
			return CreatedAtAction(nameof(GetCountry),
				new
				{
					id = country.Id,
					name = country.Name,
					iso = country.Iso,
					currency = country.Currency,
					continent = country.Continent.Name,
				},
				Mapper.Map<Country, CountryDto>(country));
		}

		/// <summary>
		/// Get a single Country.
		/// </summary>
		/// <param name="id">Country's unique ID.</param>
		/// <response code="200">Returns single Country.</response>
		/// <response code="404">Country not found.</response>
		/// <returns>Single <see cref="CountryDto"/>.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<CountryDto>> GetCountry(long id)
		{
			var country = await Context.Countries.SingleOrDefaultAsync(x => x.Id == id);

			if (country == null)
			{
				return NotFound("Country not found");
			}

			return Ok(Mapper.Map<Country, CountryDto>(country));
		}

		/// <summary>
		/// Deletes a Country.
		/// </summary>
		/// <param name="addCountry">Country information.</param>
		/// <response code="204">Country deleted.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">Country not found.</response>
		/// <returns>No Content.</returns>
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
			return NoContent();
		}
	}
}
