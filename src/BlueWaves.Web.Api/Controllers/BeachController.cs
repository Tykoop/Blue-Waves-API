namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Authorization;
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

		/// <summary>
		/// Gets all Beachess.
		/// </summary>
		/// <returns></returns>
		[HttpGet("")]
		// [AllowAnonymous]
		public async Task<ActionResult<PagedResult<BeachDto>>> GetMovies([PositiveNumberValidator] int page, [ItemPerPageValidator] int itemsPerPage)
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
			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: token);
			var result = new BeachDto(Id: beach.Id, Name: beach.Name, Latitude: beach.Coordinates.X, Longtitude: beach.Coordinates.Y, Description: beach.Description);
			Logger.LogInformation(BWLogTemplates.RequestEntity, nameof(Beach), id);
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
