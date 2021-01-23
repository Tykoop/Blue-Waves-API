namespace Esentis.BlueWaves.Persistence.Model
{
	using System;

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Identity;

	using Kritikos.Configuration.Persistence.Abstractions;

	public class Favorite : ITimestamped
	{
		public Beach Beach { get; set; }

		public BlueWavesUser User { get; set; }

		#region Implementation of ITimestamped
		/// <inheritdoc />
		public DateTimeOffset CreatedAt { get; set; }

		/// <inheritdoc />
		public DateTimeOffset UpdatedAt { get; set; }
		#endregion
	}
}
