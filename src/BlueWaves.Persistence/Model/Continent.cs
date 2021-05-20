namespace Esentis.BlueWaves.Persistence.Model
{
	using Esentis.BlueWaves.Persistence.Helpers;

	public class Continent : BlueWavesEntity<long>
	{
		public string Name { get; set; }

		public long Size { get; set; }

	}
}
