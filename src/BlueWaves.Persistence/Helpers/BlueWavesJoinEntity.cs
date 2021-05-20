namespace Esentis.BlueWaves.Persistence.Helpers
{
	using System;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	public abstract class BlueWavesJoinEntity : IAuditable<Guid>, ITimestamped
	{
		public Guid CreatedBy { get; set; }

		public Guid UpdatedBy { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}
