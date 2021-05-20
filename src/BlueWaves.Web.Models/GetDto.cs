namespace Esentis.BlueWaves.Web.Models
{
	using System;

	public record RefreshTokenDto(string accessToken, string refreshToken);

	public record AddBeachDto(string Name, double Longtitude, double Latitude, string Description, long countryId);

	public record AddRatingDto(double Rate, long BeachId, string Review);

	public record AddCountryDto(string Name, string Code, string Currency, string Description, long continentId);

	public record AddContinentDto(string Name, long Size);
}
