namespace Esentis.BlueWaves.Persistence.Model
{
	using System;

	using Esentis.BlueWaves.Persistence.Helpers;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	public class Image : BlueWavesEntity<long>,ISoftDeletable
	{
		public Beach Beach { get; set; }

		public Uri Url { get; set; }

		public bool IsDeleted { get; set; }
	}
}
