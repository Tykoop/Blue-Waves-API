namespace Esentis.BlueWaves.Web.Models
{
	public record BeachDto(long Id, string Name, double Longtitude, double Latitude, string Description);

	public record AddBeachDto(string Name, double Longtitude, double Latitude, string Description);

	public record AddRatingDto(int Rate, long BeachId);
}
