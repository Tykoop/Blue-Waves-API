namespace Esentis.BlueWaves.Web.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public record UserRegisterDto(
		[Required] string UserName,
		[Required][EmailAddress] string Email,
		[Required][PasswordPropertyText] string Password);

	public record UserLoginDto(
		[Required] string UserName,
		[Required][PasswordPropertyText] string Password);
}
