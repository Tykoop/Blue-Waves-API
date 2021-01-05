namespace Esentis.BlueWaves.Web.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Linq;
	using System.Security.Claims;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Api.Options;
	using Esentis.BlueWaves.Web.Models;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
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
			RoleManager<BlueWavesRole> roleManager, UserManager<BlueWavesUser> userManager,
			IOptions<JwtOptions> options)
			: base(logger, ctx)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			jwtOptions = options.Value;
		}

		[HttpPost("")]
		[AllowAnonymous]
		public async Task<ActionResult> RegisterUser([FromBody] UserRegisterDto userRegister)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
			}

			var user = new BlueWavesUser { Email = userRegister.Email, UserName = userRegister.UserName, };
			var result = await userManager.CreateAsync(user, userRegister.Password);
			return !result.Succeeded
				? Conflict(result.Errors)
				: Ok();
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<string>> LoginUser([FromBody] UserLoginDto userLogin)
		{
			var user = await userManager.FindByNameAsync(userLogin.UserName)
						?? await userManager.FindByEmailAsync(userLogin.UserName);
			if (user == null || !await userManager.CheckPasswordAsync(user, userLogin.Password))
			{
				return NotFound("User not found or wrong password");
			}

			var expiration = DateTimeOffset.UtcNow.AddDays(30);
			var claims = await GenerateClaims(user);
			var token = GenerateJwt(claims, expiration);
			return Ok(token);
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
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("veryverybigverysecretkey"));
			var token = new JwtSecurityToken(issuer: "BlueWaves", audience: "BlueWaves",
				notBefore: DateTimeOffset.UtcNow.UtcDateTime, expires: expiration.UtcDateTime, claims: claims,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
