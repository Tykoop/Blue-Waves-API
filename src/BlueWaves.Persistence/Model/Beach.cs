namespace Esentis.BlueWaves.Persistence.Model
{

	using Esentis.BlueWaves.Persistence.Base;

	using NetTopologySuite.Geometries;

	public class Beach : Entity<long>
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public Point Coordinates { get; set; }

	}
}
