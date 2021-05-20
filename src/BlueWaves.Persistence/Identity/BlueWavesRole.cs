namespace Esentis.BlueWaves.Persistence.Identity
{
	using System;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	using Microsoft.AspNetCore.Identity;

	public class BlueWavesRole : IdentityRole<Guid>, IEntity<Guid>, ITimestamped
	{
		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}
