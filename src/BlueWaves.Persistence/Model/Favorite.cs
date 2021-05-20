namespace Esentis.BlueWaves.Persistence.Model
{
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	public class Favorite : BlueWavesEntity<long>, ISoftDeletable
	{
		public Beach Beach { get; set; }

		public BlueWavesUser User { get; set; }

		public bool IsDeleted { get; set; }
	}
}
