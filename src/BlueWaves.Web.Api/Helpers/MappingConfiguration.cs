namespace Esentis.BlueWaves.Persistence.Helpers
{
	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Models.Dto;

	using Kritikos.PureMap;
	using Kritikos.PureMap.Contracts;

	public static class MappingConfiguration
	{
		public static readonly IPureMapperConfig Mapping = new PureMapperConfig()
			.Map<Beach, BeachDto>(mapper => beach => new BeachDto()
			{
				Description = beach.Description,
				Id = beach.Id,
				Latitude = beach.Coordinates.X,
				Longtitude = beach.Coordinates.Y,
				Name = beach.Name,
				AverageRating = beach.AverageRating,
				CountryName = beach.Country.Name,
			})
			.Map<Favorite, FavoriteDto>(_ => favorite => new FavoriteDto()
				{
					BeachId = favorite.Beach.Id,
					BeachName = favorite.Beach.Name,
					CreatedAt = favorite.Beach.CreatedAt,
					Id = favorite.Id,
				}
			)
			.Map<Rating, RatingDto>(_ => rating => new RatingDto()
			{
				BeachId = rating.Beach.Id, BeachName = rating.Beach.Name, Rate = rating.Rate, Review = rating.Review,
			})
			.Map<Image, ImageDto>(_ => image => new ImageDto() { Id = image.Id, Url = image.Url.ToString(), })
			.Map<Continent, ContinentDto>(_ => continent => new ContinentDto()
			{
				Id = continent.Id, Name = continent.Name, Size = continent.Size,
			})
			.Map<Country, CountryDto>(_ => country => new CountryDto()
			{
				Id = country.Id, Name = country.Name, Iso = country.Iso,
			});
	}
}
