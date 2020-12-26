namespace Esentis.BlueWaves.Persistence.Model
{
	using System.ComponentModel.DataAnnotations;

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Identity;

	public class Rating : Entity<long>
	{
		public Beach Beach { get; set; }

		public BlueWavesUser User { get; set; }

		[Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		public int Rate { get; set; }

	}
}
