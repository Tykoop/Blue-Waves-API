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
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[Route("api/rating")]
	public class RatingController : BaseController<RatingController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public RatingController(ILogger<RatingController> logger, BlueWavesDbContext ctx,
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager)
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
			: base(logger, ctx)
		{
			this.userManager = userManager;
		}

		[HttpPost("add")]
		public async Task<ActionResult> AddRating(AddRatingDto addRatingDto, CancellationToken token = default)
		{
			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = await Context.Beaches.FirstOrDefaultAsync(x => x.Id == addRatingDto.BeachId, cancellationToken: token);
			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			var rating = new Rating { Beach = beach, User = user, Rate = addRatingDto.Rate };
			await Context.Ratings.AddAsync(rating, cancellationToken: token);

			await Context.SaveChangesAsync(cancellationToken: token);
			return Ok("Beach successfuly rated");
		}

		[HttpGet("")]
		public async Task<ActionResult<List<Rating>>> PersonalRatings()
		{
			var userId = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var ratings = await Context.Ratings.Where(x => x.User.Id == user.Id).ToListAsync();
			return Ok(ratings);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> RemoveRating(long id)
		{
			var rating = await Context.Ratings.SingleOrDefaultAsync(x => x.Id == id);
			if (rating == null)
			{
				Logger.LogInformation(BWLogTemplates.NotFound, nameof(Rating));
				return NotFound("Rating not found");
			}

			Context.Ratings.Remove(rating);
			await Context.SaveChangesAsync();
			Logger.LogInformation(BWLogTemplates.Deleted, nameof(Rating), id);
			return Ok("Rating successfully deleted");
		}
	}
}
