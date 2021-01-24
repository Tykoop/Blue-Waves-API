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
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var beach = await Context.Beaches.FirstOrDefaultAsync(x => x.Id == addRatingDto.BeachId, token);
			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			var rating = await Context.Ratings.IgnoreQueryFilters()
				.SingleOrDefaultAsync(x => x.User == user && x.Beach == beach, token);

			// If user is trying to add a rating that he has already added and deleted recently
			if (rating != null && rating.IsDeleted && rating.Rate == addRatingDto.Rate)
			{
				rating.IsDeleted = false;
				await Context.SaveChangesAsync(token);
				return Ok("Beach rated");
			}

			rating = new Rating { Beach = beach, User = user, Rate = addRatingDto.Rate };
			Context.Ratings.Add(rating);

			await Context.SaveChangesAsync(token);
			return Ok("Beach successfuly rated");
		}

		[HttpDelete("delete")]
		public async Task<ActionResult> RemoveRating(long beachId, CancellationToken token = default)
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

			var rating =
				await Context.Ratings.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id);
			if (rating == null)
			{
				return BadRequest("Beach not found");
			}

			rating.IsDeleted = true;
			await Context.SaveChangesAsync();
			return Ok("Favorited deleted");
		}

		[HttpGet("")]
		public async Task<ActionResult<List<Rating>>> PersonalRatings([PositiveNumberValidator] int page, [ItemPerPageValidator] int itemsPerPage)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("Something went wrong");
			}

			var toSkip = itemsPerPage * (page - 1);
			var ratings = Context.Ratings.Include(x => x.Beach)
				.Where(x => x.User.Id == user.Id)
				.OrderBy(x => x.CreatedAt);
			var totalRatings = await ratings.CountAsync();
			var pagedRatings = await ratings
				.Skip(toSkip)
				.Take(itemsPerPage)
				.ToListAsync();
			var result = new PagedResult<RatingDto>
			{
				Results = pagedRatings.Select(x => x.toDto()).ToList(),
				Page = page,
				TotalPages = (totalRatings / itemsPerPage) + 1,
				TotalElements = totalRatings,
			};

			if (page > ((totalRatings / itemsPerPage) + 1))
			{
				return BadRequest("Page doesn't exist");
			}

			return Ok(result);

		}

		[HttpPost("check")]
		public async Task<ActionResult<int>> GetRating(long beachId, CancellationToken token = default)
		{
			var foundUser = Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
			if (foundUser)
			{
				var rating = await Context.Ratings.Where(x => x.Beach.Id == beachId && x.User.Id == userId).Select(x => x.Rate).FirstOrDefaultAsync();
				if (rating == 0)
				{
					return -1;
				}

				return rating;
			}

			return BadRequest("Something went wrong");
		}
	}
}
