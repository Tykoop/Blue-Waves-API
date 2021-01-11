namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;

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

		[HttpPost("")]
		public async Task<ActionResult> ToggleFavorite(long beachId, CancellationToken token = default)
		{
			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
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

			var favorite = await Context.Favorites.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id);
			if (favorite == null)
			{
				favorite = new Favorite { Beach = beach, User = user };
				await Context.Favorites.AddAsync(favorite, token);
				await Context.SaveChangesAsync();
				return Ok("Beach added to favorites");
			}

			Context.Favorites.Remove(favorite);
			await Context.SaveChangesAsync();
			return Ok("Beach removed from favorites");
		}

		[HttpPost("check")]
		public async Task<ActionResult<Rating>> IsFavorited(long beachId, CancellationToken token = default)
		{
			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = await Context.Beaches.FirstOrDefaultAsync(x => x.Id == beachId, token);
			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			var rating = await Context.Favorites.SingleOrDefaultAsync(x => x.User == user && x.Beach == beach);

			if (rating == null)
			{
				return NotFound("Beach is not favorited");
			}

			return Ok("Beach is favorited");
		}
	}
}
