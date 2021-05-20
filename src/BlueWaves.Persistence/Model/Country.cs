namespace Esentis.BlueWaves.Persistence.Model
{
	using Esentis.BlueWaves.Persistence.Helpers;

	public class Country : BlueWavesEntity<long>
	{
		public string Name { get; set; }

		public string Iso { get; set; }

		public string Currency { get; set; }

		public string Description { get; set; }

		public Continent Continent { get; set; }
	}
}
