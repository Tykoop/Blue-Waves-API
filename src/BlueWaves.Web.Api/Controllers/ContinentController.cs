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

	[Route("api/continent")]
	public class ContinentController : BaseController<ContinentController>
	{
		public ContinentController(ILogger<ContinentController> logger, BlueWavesDbContext ctx, IPureMapper mapper)
			: base(logger, ctx, mapper)
		{
		}

		/// <summary>
		/// Returns all Continents.
		/// </summary>
		/// <param name="criteria">Paging criteria.</param>
		/// <response code="200">Gets all continents.</response>
		/// <returns>List of <see cref="ContinentDto"/>.</returns>
		[HttpPost("all")]
		public async Task<ActionResult<PagedResult<ContinentDto>>> GetAllContinents(PaginationCriteria criteria,
			CancellationToken token = default)
		{
			var continents = Context.Continents.OrderBy(x => x.Id);

			var count = await continents.CountAsync(token);

			var slice = await continents.Slice(criteria.Page, criteria.ItemsPerPage)
				.Project<Continent, ContinentDto>(Mapper)
				.ToListAsync(token);

			var result = new PagedResult<ContinentDto>
			{
				Results = slice,
				Page = criteria.Page,
				TotalElements = count,
				TotalPages = (count / criteria.ItemsPerPage) + 1,
			};

			return Ok(result);
		}

		/// <summary>
		/// Add a Continent.
		/// </summary>
		/// <param name="addContinentDto">Continent information.</param>
		/// <response code="201">Added successfully.</response>
		/// <response code="400">Missing fields.</response>
		/// <response code="401">User not authorized.</response>
		/// <returns>Created <see cref="ContinentDto"/>.</returns>
		[HttpPost("add")]
		public async Task<ActionResult> AddContinent(AddContinentDto addContinentDto, CancellationToken token = default)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState.Values.Select(v => v.Errors));
			}

			var continent = new Continent { Name = addContinentDto.Name, Size = addContinentDto.Size };
			await Context.Continents.AddAsync(continent, token);
			await Context.SaveChangesAsync(token);
			return CreatedAtAction(nameof(GetContinent),
				new { id = continent.Id, name = continent.Name, size = continent.Size, },
				Mapper.Map<Continent, ContinentDto>(continent));
		}

		/// <summary>
		/// Get a single Continent.
		/// </summary>
		/// <param name="id">Continent's unique ID.</param>
		/// <response code="200">Returns a Continent.</response>
		/// <response code="404">Continent not found.</response>
		/// <returns>Single <see cref="ContinentDto"/>.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ContinentDto>> GetContinent(long id, CancellationToken token = default)
		{
			var continent = await Context.Continents.SingleOrDefaultAsync(c => c.Id == id, token);

			if (continent == null)
			{
				return NotFound("Continent not found");
			}

			return Ok(Mapper.Map<Continent, ContinentDto>(continent));
		}

		/// <summary>
		/// Delete a Continent.
		/// </summary>
		/// <param name="id">Continent's unique ID.</param>
		/// <response code="204">Deleted successfully.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">Continent not found.</response>
		/// <returns>No Content.</returns>
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
			return NoContent();
		}
	}
}
