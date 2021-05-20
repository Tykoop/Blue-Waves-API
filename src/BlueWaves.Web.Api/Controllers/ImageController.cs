namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Models.Dto;

	using Kritikos.PureMap.Contracts;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	[AllowAnonymous]
	[Route("api/image")]
	public class ImageController : BaseController<ImageController>
	{
		private readonly UserManager<BlueWavesUser> userManager;

		public ImageController(ILogger<ImageController> logger, BlueWavesDbContext ctx, IPureMapper mapper,
			UserManager<BlueWavesUser> userManager)
			: base(logger, ctx, mapper)
			=> this.userManager = userManager;

		/// <summary>
		/// Add an Image associated to Beach.
		/// </summary>
		/// <param name="dto">Image information.</param>
		/// <response code="204">Added to favorites.</response>
		/// <response code="400">Validation errors.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">User not found. Beach not found.</response>
		/// <returns>No Content.</returns>
		[HttpPost("")]
		public async Task<ActionResult<ImageDto>> AddImage(AddImageDto dto, CancellationToken token = default)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState.Values.Select(x => x.Errors));
			}

			var userId = RetrieveUserId().ToString();
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			var beach = await Context.Beaches.SingleOrDefaultAsync(b => b.Id == dto.BeachId, default);

			if (beach == null)
			{
				return NotFound("Beach not found");
			}

			var images = new List<Image>();

			foreach (var url in dto.Images)
			{
				images.Add(new Image { Beach = beach, Url = new Uri(url), });
			}

			Context.Images.AddRange(images);
			await Context.SaveChangesAsync(token);
			Logger.LogInformation(BWLogTemplates.CreatedEntities, images.Count, nameof(Image));
			return Ok(images.Select(i => Mapper.Map<Image, ImageDto>(i)));
		}
	}
}
