namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
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

	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[Route("api/favorite")]
	public class FavoriteController : BaseController<FavoriteController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public FavoriteController(ILogger<FavoriteController> logger, BlueWavesDbContext ctx,
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager, IPureMapper mapper)
			: base(logger, ctx, mapper) =>
			this.userManager = userManager;

		/// <summary>
		/// Add a Beach to Favorites.
		/// </summary>
		/// <param name="beachId">Beach's unique ID.</param>
		/// <response code="204">Added to favorites.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found.</response>
		/// <returns>No Content.</returns>
		[HttpPost("add")]
		public async Task<ActionResult> AddFavorite(long beachId, CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == beachId, token);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			var favorite =
				await Context.Favorites.IgnoreQueryFilters()
					.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id, token);
			if (favorite == null)
			{
				favorite = new Favorite { Beach = beach, User = user };
				await Context.Favorites.AddAsync(favorite, token);
				await Context.SaveChangesAsync(token);
				return NoContent();
			}

			if (favorite.IsDeleted)
			{
				favorite.IsDeleted = false;
				await Context.SaveChangesAsync(token);
				return NoContent();
			}

			Context.Favorites.Add(favorite);
			await Context.SaveChangesAsync(token);
			return NoContent();
		}

		/// <summary>
		/// Deletes a Favorite.
		/// </summary>
		/// <param name="beachId">Beach's unique ID.</param>
		/// <response code="204">Removed from favorites.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found. Favorite not found.</response>
		/// <returns>No Content.</returns>
		[HttpDelete("delete")]
		public async Task<ActionResult> RemoveFavorite(long beachId, CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User went wrong");
			}

			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == beachId, token);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			var favorite =
				await Context.Favorites.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id, token);
			if (favorite == null)
			{
				return NotFound("Favorite not found");
			}

			favorite.IsDeleted = true;
			await Context.SaveChangesAsync(token);
			return Ok("Favorited deleted");
		}

		/// <summary>
		/// Returns User's personal favorites.
		/// </summary>
		/// <param name="criteria">Paging criteria.</param>
		/// <response code="200">List of favorites.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found. Favorite not found.</response>
		/// <returns>List of <see cref="FavoriteDto"/>.</returns>
		[HttpPost("personal")]
		public async Task<ActionResult<PagedResult<FavoriteDto>>> PersonalFavorites(PaginationCriteria criteria,
			CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var favorites = Context.Favorites.Include(x => x.Beach)
				.Where(x => x.User.Id == user.Id)
				.OrderBy(x => x.Id);

			var totalFavorites = await favorites.CountAsync(token);

			var slice = await favorites.Slice(criteria.Page, criteria.ItemsPerPage)
				.Project<Favorite, FavoriteDto>(Mapper)
				.ToListAsync(token);

			var result = new PagedResult<FavoriteDto>
			{
				Results = slice,
				Page = criteria.Page,
				TotalPages = (totalFavorites / criteria.ItemsPerPage) + 1,
				TotalElements = totalFavorites,
			};
			return Ok(result);
		}

		/// <summary>
		/// Checks whether user has already favorited a Beach.
		/// </summary>
		/// <param name="beachId">Beach's unique ID.</param>
		/// <returns>True or False.</returns>
		[HttpPost("check")]
		public async Task<bool> IsFavorited(long beachId, CancellationToken token = default)
		{
			var foundUser = Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
			return foundUser
					&& await Context.Favorites.AnyAsync(x => x.Beach.Id == beachId && x.User.Id == userId, token);
		}
	}
}
