namespace Esentis.BlueWaves.Persistence.Model
{

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Identity;

	public class Favorite : Entity<long>
	{
		public Beach Beach { get; set; }

		public BlueWavesUser User { get; set; }
	}
}
