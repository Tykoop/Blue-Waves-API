namespace Esentis.BlueWaves.Persistence.Model
{
	using System;
	using System.ComponentModel.DataAnnotations;

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;

	using Kritikos.Configuration.Persistence.Abstractions;

	public class Rating : ITimestamped, IDeletable
	{
		public Beach Beach { get; set; }

		public BlueWavesUser User { get; set; }

		public int Rate { get; set; }

		#region Implementation of ITimestamped
		/// <inheritdoc />
		public DateTimeOffset CreatedAt { get; set; }

		/// <inheritdoc />
		public DateTimeOffset UpdatedAt { get; set; }
		#endregion

		#region Implementation of IDeletable

		/// <inheritdoc />
		public bool IsDeleted { get; set; }
		#endregion
	}
}
