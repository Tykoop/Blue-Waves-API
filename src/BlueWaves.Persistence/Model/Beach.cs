namespace Esentis.BlueWaves.Persistence.Model
{
	using System.Collections.Generic;

	using Esentis.BlueWaves.Persistence.Helpers;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	using NetTopologySuite.Geometries;

	public class Beach : BlueWavesEntity<long>, ISoftDeletable
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public Point Coordinates { get; set; }

		public Country Country { get; set; }

		public bool IsDeleted { get; set; }

		public double AverageRating { get; set; }

		public IReadOnlyCollection<Rating> Ratings { get; set; }
			= new List<Rating>(0);

		public IReadOnlyCollection<Image> Images { get; set; }
			= new List<Image>(0);
	}
}
