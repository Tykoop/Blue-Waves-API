namespace Esentis.BlueWaves.Persistence.Identity
{
	using System;

	using Esentis.BlueWaves.Persistence.Helpers;

	public class Device : BlueWavesEntity<long>
	{
		public string Name { get; set; }

		public BlueWavesUser User { get; set; }

		public Guid RefreshToken { get; set; }
	}
}
