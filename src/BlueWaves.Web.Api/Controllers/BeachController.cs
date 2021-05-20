namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;
	using Esentis.BlueWaves.Web.Models.Dto;
	using Esentis.Ieemdb.Web.Models.SearchCriteria;

	using Kritikos.Extensions.Linq;
	using Kritikos.PureMap;
	using Kritikos.PureMap.Contracts;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using NetTopologySuite.Geometries;

	[Route("api/beach")]
	public class BeachController : BaseController<BeachController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public BeachController(ILogger<BeachController> logger, BlueWavesDbContext ctx,
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager,
			IPureMapper mapper)
			: base(logger, ctx, mapper) =>
			this.userManager = userManager;

		/// <summary>
		/// Returns all Beaches.
		/// </summary>
		/// <param name="criteria">Paging criteria.</param>
		/// <response code="200">Returns list of beaches.</response>
		/// <returns>List of <see cref="BeachDto"/>.</returns>
		[AllowAnonymous]
		[HttpPost("all")]
		public async Task<ActionResult<PagedResult<BeachDto>>> GetBeaches(PaginationCriteria criteria,
			CancellationToken token = default)
		{
			var beaches = Context.Beaches
				.TagWith($"Retrieving all beaches. Current page {criteria.Page}.")
				.OrderBy(x => x.Id);

			var totalBeaches = await beaches.CountAsync(token);

			var slice = await beaches.Slice(criteria.Page, criteria.ItemsPerPage)
				.Project<Beach, BeachDto>(Mapper)
				.ToListAsync(token);

			var result = new PagedResult<BeachDto>
			{
				Results = slice,
				Page = criteria.Page,
				TotalPages = (totalBeaches / criteria.ItemsPerPage) + 1,
				TotalElements = totalBeaches,
			};
			Logger.LogInformation(BWLogTemplates.RequestEntities, nameof(Beach), totalBeaches);
			return Ok(result);
		}

		/// <summary>
		/// Returns single Beach information.
		/// </summary>
		/// <param name="id">Beach's unique ID.</param>
		/// <response code="200">Returns a single beach.</response>
		/// <response code="404">Beach not found.</response>
		/// <returns>Single <see cref="BeachDto"/>.</returns>
		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<BeachDto>> GetBeach(long id, CancellationToken token = default)
		{
			var beach = await Context.Beaches.Include(b => b.Country)
				.TagWith("Requesting beach")
				.SingleOrDefaultAsync(x => x.Id == id, token);

			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			Logger.LogInformation(BWLogTemplates.RequestEntity, nameof(Beach), id);
			return Ok(Mapper.Map<Beach, BeachDto>(beach));
		}

		/// <summary>
		/// Adds a new Beach.
		/// </summary>
		/// <param name="addBeachDto">Beach details.</param>
		/// <response code="200">Returns a single beach.</response>
		/// <response code="401">User not Authorized.</response>
		/// <response code="400">Missing information.</response>
		/// <response code="404">User not found. Country not found.</response>
		/// <returns>Created <see cref="BeachDto"/>.</returns>
		[HttpPost("add")]
		public async Task<ActionResult<BeachDto>> AddBeach(AddBeachDto addBeachDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
			}

			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var country = await Context.Countries.SingleOrDefaultAsync(c => c.Id == addBeachDto.countryId);
			if (country == null)
			{
				return NotFound("Country not found");
			}

			var beach = new Beach
			{
				Coordinates = new Point(addBeachDto.Latitude, addBeachDto.Longtitude),
				Name = addBeachDto.Name,
				Description = addBeachDto.Description,
				Country = country,
			};
			Context.Beaches.Add(beach);
			await Context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetBeach),
				new
				{
					id = beach.Id, name = beach.Name, description = beach.Description, coordinates = beach.Coordinates,
				},
				Mapper.Map<Beach, BeachDto>(beach));
		}

		/// <summary>
		/// Deletes a Beach.
		/// </summary>
		/// <param name="id">Beach's unique ID.</param>
		/// <response code="204">Beach deleted.</response>
		/// <response code="401">User not Authorized.</response>
		/// <response code="404">Beach not found.</response>
		/// <returns>No content.</returns>
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteBeach(long id)
		{
			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == id);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			await Context.Ratings.Where(x => x.Beach == beach).ForEachAsync(x => x.IsDeleted = true);
			await Context.Favorites.Where(x => x.Beach == beach).ForEachAsync(x => x.IsDeleted = true);
			beach.IsDeleted = true;
			await Context.SaveChangesAsync();
			Logger.LogInformation(BWLogTemplates.Deleted, nameof(Beach), id);
			return Ok("Beach deleted");
		}

		/// <summary>
		/// Restores a Beach.
		/// </summary>
		/// <param name="id">Beach's unique ID.</param>
		/// <response code="204">Beach restored.</response>
		/// <response code="401">User not authorized.</response>
		/// <returns>No content.</returns>
		[HttpPost("restore/{id}")]
		public async Task<ActionResult> UndeleteBeach(long id)
		{
			var beach = await Context.Beaches
				.IgnoreQueryFilters()
				.SingleOrDefaultAsync(x => x.Id == id);

			return NoContent();
		}

		/// <summary>
		/// Returns all Images associated with the Beach.
		/// </summary>
		/// <param name="id">Beach's unique ID.</param>
		/// <response code="200">List of images.</response>
		/// <response code="401">User not authorized.</response>
		/// <returns>List of <see cref="ImageDto"/>.</returns>
		[AllowAnonymous]
		[HttpGet("{id}/images")]
		public async Task<ActionResult<List<ImageDto>>> GetImages(long id, CancellationToken token = default)
		{
			var images = await Context.Images.Where(i => i.Beach.Id == id)
				.Project<Image, ImageDto>(Mapper)
				.ToListAsync(token);

			return Ok(images);
		}
	}
}
