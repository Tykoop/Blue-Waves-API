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

	[Route("api/rating")]
	public class RatingController : BaseController<RatingController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public RatingController(ILogger<RatingController> logger, BlueWavesDbContext ctx,
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager, IPureMapper mapper)
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
			: base(logger, ctx, mapper)
		{
			this.userManager = userManager;
		}

		/// <summary>
		/// Add a Rating to Beach.
		/// </summary>
		/// <param name="addRatingDto">Rating information.</param>
		/// <response code="201">Rating added.</response>
		/// <response code="400">Validation errors.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found.</response>
		/// <response code="409">Beach already rated.</response>
		/// <returns>Created <see cref="RatingDto"/>.</returns>
		[HttpPost("add")]
		public async Task<ActionResult<RatingDto>> AddRating(AddRatingDto addRatingDto,
			CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var beach = await Context.Beaches.FirstOrDefaultAsync(x => x.Id == addRatingDto.BeachId, token);
			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			var rating = await Context.Ratings
				.SingleOrDefaultAsync(x => x.User == user && x.Beach == beach, token);

			if (rating != null)
			{
				return Conflict("Beach already rated");
			}

			beach.AverageRating = (beach.Ratings.Sum(x => x.Rate) + addRatingDto.Rate) / (beach.Ratings.Count + 1);
			rating = new Rating { Beach = beach, User = user, Rate = addRatingDto.Rate, Review = addRatingDto.Review };
			Context.Ratings.Add(rating);

			await Context.SaveChangesAsync(token);

			return CreatedAtAction(nameof(GetRating),
				new
				{
					rate = rating.Rate,
					createdAt = rating.CreatedAt,
					beachName = rating.Beach.Name,
					beachId = rating.Beach.Id,
				},
				Mapper.Map<Rating, RatingDto>(rating));
		}

		/// <summary>
		/// Delete a Rating.
		/// </summary>
		/// <param name="beachId">Beach's unique ID.</param>
		/// <response code="204">Rating removed.</response>
		/// <response code="400">Validation errors.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found. Rating not found.</response>
		/// <returns>No Content.</returns>
		[HttpDelete("delete")]
		public async Task<ActionResult> RemoveRating(long beachId, CancellationToken token = default)
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

			var rating =
				await Context.Ratings.SingleOrDefaultAsync(x => x.Beach.Id == beachId && x.User.Id == user.Id, token);
			if (rating == null)
			{
				return NotFound("Rating not found");
			}

			if ((rating.Beach.Ratings.Count - 1) <= 0)
			{
				rating.Beach.AverageRating = 0;
			}
			else
			{
				rating.Beach.AverageRating = rating.Beach.Ratings.Where(x => x.Id != rating.Id).Sum(x => x.Rate)
											/ (rating.Beach.Ratings.Count - 1);
			}

			Context.Ratings.Remove(rating);
			await Context.SaveChangesAsync(token);
			return NoContent();
		}

		/// <summary>
		/// Returns user's personal ratings.
		/// </summary>
		/// <param name="criteria">Paging criteria.</param>
		/// <response code="200">Returns list of ratings.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found. Rating not found.</response>
		/// <returns>List of <see cref="RatingDto"/>.</returns>
		[HttpPost("personal")]
		public async Task<ActionResult<PagedResult<RatingDto>>> PersonalRatings(PaginationCriteria criteria,
			CancellationToken token = default)
		{
			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var ratings = Context.Ratings.Include(x => x.Beach)
				.Where(x => x.User.Id == user.Id)
				.OrderBy(x => x.CreatedAt);

			var totalRatings = await ratings.CountAsync(token);

			var pagedRatings = await ratings.Slice(criteria.Page, criteria.ItemsPerPage)
				.Project<Rating, RatingDto>(Mapper)
				.ToListAsync(token);
			var result = new PagedResult<RatingDto>
			{
				Results = pagedRatings,
				Page = criteria.Page,
				TotalPages = (totalRatings / criteria.ItemsPerPage) + 1,
				TotalElements = totalRatings,
			};

			return Ok(result);
		}

		/// <summary>
		/// Check if user has rated a specific beach.
		/// </summary>
		/// <param name="beachId">Beach's unique ID.</param>
		/// <response code="200">Returns list of ratings.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Rating not found.</response>
		/// <returns>Single <see cref="RatingDto"/>.</returns>
		[HttpPost("check")]
		public async Task<ActionResult<RatingDto>> GetRating(long beachId, CancellationToken token = default)
		{
			var foundUser = Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
			if (foundUser)
			{
				var rating = await Context.Ratings.Where(x => x.Beach.Id == beachId && x.User.Id == userId)
					.Project<Rating, RatingDto>(Mapper)
					.FirstOrDefaultAsync(token);
				if (rating == null)
				{
					return NotFound("Rating not found");
				}

				return Ok(rating);
			}

			return NotFound("User not found");
		}
	}
}
