namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using Microsoft.Extensions.Logging;

	using NetTopologySuite.Geometries;

	[Route("api/beach")]
	public class BeachController : BaseController<BeachController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public BeachController(ILogger<BeachController> logger, BlueWavesDbContext ctx,
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager, IMemoryCache cache)
			: base(logger, ctx)
		{
			//cache.Set("kaitoula", 3);
			this.userManager = userManager;
		}

		/// <summary>
		/// Gets all Beachess.
		/// </summary>
		/// <returns></returns>
		[HttpGet("")]

		// [AllowAnonymous]
		public async Task<ActionResult<PagedResult<BeachDto>>> GetMovies([PositiveNumberValidator] int page,
			[ItemPerPageValidator] int itemsPerPage)
		{
			var user = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var toSkip = itemsPerPage * (page - 1);
			var beaches = Context.Beaches
				.TagWith($"Retrieving all beaches for user {user}")
				.OrderBy(x => x.Id);

			var totalBeaches = await beaches.CountAsync();
			if (page > ((totalBeaches / itemsPerPage) + 1))
			{
				return BadRequest("Page doesn't exist");
			}

			var pagedBeaches = await beaches
				.Skip(toSkip)
				.Take(itemsPerPage)
				.ToListAsync();
			var result = new PagedResult<BeachDto>
			{
				Results = pagedBeaches.Select(x => x.toDto()).ToList(),
				Page = page,
				TotalPages = (totalBeaches / itemsPerPage) + 1,
				TotalElements = totalBeaches,
			};
			Logger.LogInformation(BWLogTemplates.RequestEntities, nameof(Beach), totalBeaches);
			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Beach>> GetBeach(long id, CancellationToken token = default)
		{
			var beach = await Context.Beaches
				.TagWith("Requesting beach")
				.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: token);

			var result = new BeachDto(Id: beach.Id, Name: beach.Name, Latitude: beach.Coordinates.X,
				Longtitude: beach.Coordinates.Y, Description: beach.Description);
			Logger.LogInformation(BWLogTemplates.RequestEntity, nameof(Beach), id);
			return beach == null
				? NotFound()
				: Ok(result);
		}

		[HttpPost("add")]
		public async Task<ActionResult<BeachDto>> AddBeach(AddBeachDto addBeachDto)
		{
			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = new Beach
			{
				Coordinates = new Point(addBeachDto.Latitude, addBeachDto.Longtitude),
				Name = addBeachDto.Name,
				Description = addBeachDto.Description
			};
			Context.Beaches.Add(beach);
			await Context.SaveChangesAsync();
			return Ok(beach.toDto());
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteBeach(long id)
		{
			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == id);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			// Flags all ratings and favorites associated with the Beach, for deletion
			await Context.Ratings.Where(x => x.Beach == beach).ForEachAsync(x => x.IsDeleted = true);
			await Context.Favorites.Where(x => x.Beach == beach).ForEachAsync(x => x.IsDeleted = true);
			beach.IsDeleted = true;
			await Context.SaveChangesAsync();
			Logger.LogInformation(BWLogTemplates.Deleted, nameof(Beach), id);
			return Ok("Beach deleted");
		}

		[HttpPost("{id}")]
		public async Task<ActionResult> UndeleteBeach(long id)
		{
			var beach = await Context.Beaches
				.IgnoreQueryFilters()
				.SingleOrDefaultAsync(x => x.Id == id);

			return Ok();
		}
	}
}
