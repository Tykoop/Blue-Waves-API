namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Linq;
	using System.Security.Claims;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Api.Options;
	using Esentis.BlueWaves.Web.Models;

	using Kritikos.PureMap.Contracts;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Microsoft.IdentityModel.Tokens;

	using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

	[Route("api/account")]
	public class AccountController : BaseController<AccountController>
	{
		private readonly RoleManager<BlueWavesRole> roleManager;
		private readonly UserManager<BlueWavesUser> userManager;
		private readonly JwtOptions jwtOptions;

		public AccountController(ILogger<AccountController> logger, BlueWavesDbContext ctx,
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager, IPureMapper mapper,
			IOptions<JwtOptions> options)
			: base(logger, ctx, mapper)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			jwtOptions = options.Value;
		}

		/// <summary>
		/// Register User.
		/// </summary>
		/// <param name="userRegister">Registration information.</param>
		/// <response code="204">Added successfully.</response>
		/// <response code="400">Validation errors.</response>
		/// <response code="409">User already exists.</response>
		/// <returns>No Content.</returns>
		[HttpPost("")]
		[AllowAnonymous]
		public async Task<ActionResult> RegisterUser([FromBody] UserRegisterDto userRegister)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
			}

			// roleManager.CreateAsync(new BlueWavesRole { Name = RoleNames.Administrator });
			var user = new BlueWavesUser { Email = userRegister.Email, UserName = userRegister.UserName, };
			var result = await userManager.CreateAsync(user, userRegister.Password);

			await userManager.AddToRoleAsync(user, RoleNames.Administrator);
			return !result.Succeeded
				? Conflict(result.Errors)
				: NoContent();
		}

		/// <summary>
		/// Login User.
		/// </summary>
		/// <param name="userLogin">Login information.</param>
		/// <response code="200">Returns tokens.</response>
		/// <response code="400">Validation errors.</response>
		/// <response code="404">User not found or wrong password.</response>
		/// <returns><see cref="UserBindingDto"/>.</returns>
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<UserBindingDto>> LoginUser([FromBody] UserLoginDto userLogin)
		{
			var user = await userManager.FindByNameAsync(userLogin.UserName)
						?? await userManager.FindByEmailAsync(userLogin.UserName);
			if (user == null || !await userManager.CheckPasswordAsync(user, userLogin.Password))
			{
				return NotFound("User not found or wrong password");
			}

			var device = await Context.Devices.FirstOrDefaultAsync(e =>
				e.Name == userLogin.DeviceName && e.User.UserName == userLogin.UserName);
			if (device == null)
			{
				device = new Device { User = user, Name = userLogin.DeviceName };
				Context.Devices.Add(device);
			}

			var accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.DurationInMinutes);

			var claims = await GenerateClaims(user);
			var token = GenerateJwt(claims, accessTokenExpiration);

			var refreshToken = Guid.NewGuid();
			device.RefreshToken = refreshToken;

			await Context.SaveChangesAsync();

			var dto = new UserBindingDto(token, accessTokenExpiration, refreshToken);

			return Ok(dto);
		}

		/// <summary>
		/// Delete User.
		/// </summary>
		/// <response code="204">User deleted.</response>
		/// <response code="404">User not found.</response>
		/// <returns><see cref="UserBindingDto"/>.</returns>
		[HttpPost("delete")]
		public async Task<ActionResult> DeleteUser(CancellationToken token = default)
		{
			var userId = RetrieveUserId();
			if (userId == Guid.Empty)
			{
				return NotFound("User not found");
			}

			var user = await userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				return NotFound("User not found");
			}

			var devices = await Context.Devices.Where(x => x.User == user).ToListAsync(token);
			var ratings = await Context.Ratings.Where(x => x.User == user).ToListAsync(token);
			var favorites = await Context.Favorites.Where(x => x.User == user).ToListAsync(token);

			Context.Devices.RemoveRange(devices);
			Context.Ratings.RemoveRange(ratings);
			Context.Favorites.RemoveRange(favorites);

			await userManager.DeleteAsync(user);
			await Context.SaveChangesAsync(token);
			return NoContent();
		}

		/// <summary>
		/// Refreshes user's access token.
		/// </summary>
		/// <response code="200">Returns new access token.</response>
		/// <response code="401">User not authorized.</response>
		/// <response code="404">Device not found.</response>
		/// <returns><see cref="UserBindingDto"/>.</returns>
		[HttpPost("refresh")]
		[AllowAnonymous]
		public async Task<ActionResult<UserBindingDto>> RefreshToken([FromBody] UserRefreshTokenDto dto)
		{
			var principal = GetPrincipalFromExpiredToken(dto.ExpiredToken);
			var valid = Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);

			var device = await Context.Devices.Where(x => x.RefreshToken == dto.RefreshToken && x.User.Id == userId)
				.SingleOrDefaultAsync();
			if (device
				== null)
			{
				return NotFound();
			}

			var user = await userManager.FindByIdAsync(userId.ToString());
			var accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.DurationInMinutes);

			var claims = await GenerateClaims(user);
			var token = GenerateJwt(claims, accessTokenExpiration);
			device.RefreshToken = Guid.NewGuid();
			await Context.SaveChangesAsync();

			var result = new UserBindingDto(token, accessTokenExpiration, device.RefreshToken);

			return Ok(result);
		}

		private async Task<List<Claim>> GenerateClaims(BlueWavesUser user)
		{
			var identityClaims = await userManager.GetClaimsAsync(user);
			var identityRoles = (await userManager.GetRolesAsync(user)).Select(x => new Claim(ClaimTypes.Role, x));
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
					ClaimValueTypes.Integer64),
			};
			claims.AddRange(identityClaims);
			claims.AddRange(identityRoles);
			return claims.ToList();
		}

		private string GenerateJwt(ICollection<Claim> claims, DateTimeOffset expiration)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
			var token = new JwtSecurityToken(
				issuer: jwtOptions.Issuer,
				audience: jwtOptions.Audience,
				notBefore: DateTimeOffset.UtcNow.UtcDateTime,
				expires: expiration.UtcDateTime,
				claims: claims,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidAudience = jwtOptions.Audience,
				ValidateIssuer = true,
				ValidIssuer = jwtOptions.Issuer,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
				ValidateLifetime = false,
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
					StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}
	}
}
