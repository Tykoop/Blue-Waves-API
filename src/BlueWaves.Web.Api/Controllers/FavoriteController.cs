namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
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

	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[Route("api/favorite")]
	public class FavoriteController : BaseController<FavoriteController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public FavoriteController(ILogger<FavoriteController> logger, BlueWavesDbContext ctx,
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager)
			: base(logger, ctx)
		{
			this.userManager = userManager;
		}

		[HttpPost("add")]
		public async Task<ActionResult> AddFavorite(long beachId, CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == beachId);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			var favorite =
				await Context.Favorites.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id);
			if (favorite == null)
			{
				favorite = new Favorite { Beach = beach, User = user };
				await Context.Favorites.AddAsync(favorite, token);
				await Context.SaveChangesAsync();
				return Ok("Beach added to favorites");
			}

			if (favorite.IsDeleted)
			{
				favorite.IsDeleted = false;
				await Context.SaveChangesAsync();
				return Ok("Beach added to favorites");
			}

			Context.Favorites.Add(favorite);
			await Context.SaveChangesAsync();
			return Ok("Beach added to favorites");
		}

		[HttpDelete("delete")]
		public async Task<ActionResult> RemoveFavorite(long beachId, CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = await Context.Beaches.SingleOrDefaultAsync(x => x.Id == beachId);
			if (beach == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Beach));
				return NotFound("Beach not found");
			}

			var favorite =
				await Context.Favorites.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id);
			if (favorite == null)
			{
				return BadRequest("Beach not found");
			}

			favorite.IsDeleted = true;
			await Context.SaveChangesAsync();
			return Ok("Favorited deleted");
		}

		[HttpGet("")]
		public async Task<ActionResult<List<Rating>>> PersonalFavorites([PositiveNumberValidator] int page,
			[ItemPerPageValidator] int itemsPerPage)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var toSkip = itemsPerPage * (page - 1);
			var favorites = Context.Favorites.Include(x => x.Beach)
				.Where(x => x.User.Id == user.Id);
			var totalFavorites = await favorites.CountAsync();
			var pagedFavorites = await favorites
				.Skip(toSkip)
				.Take(itemsPerPage)
				.ToListAsync();

			var result = new PagedResult<FavoriteDto>
			{
				Results = pagedFavorites.Select(x => x.toDto()).ToList(),
				Page = page,
				TotalPages = (totalFavorites / itemsPerPage) + 1,
				TotalElements = totalFavorites,
			};

			if (page > ((totalFavorites / itemsPerPage) + 1))
			{
				return BadRequest("Page doesn't exist");
			}

			return Ok(result);
		}

		[HttpPost("check")]
		public async Task<bool> IsFavorited(long beachId, CancellationToken token = default)
		{
			var foundUser = Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
			return foundUser
					&& await Context.Favorites.AnyAsync(x => x.Beach.Id == beachId && x.User.Id == userId, token);
		}
	}
}
