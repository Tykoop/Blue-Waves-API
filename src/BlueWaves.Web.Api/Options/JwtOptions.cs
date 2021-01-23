namespace Esentis.BlueWaves.Web.Api.Options
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class JwtOptions
	{
		public string Issuer { get; set; }

		public string Audience { get; set; }

		public string Key { get; set; }

		public int DurationInMinutes { get; set; }

		public int RefreshTokenDurationInDays { get; set; }
	}
}
