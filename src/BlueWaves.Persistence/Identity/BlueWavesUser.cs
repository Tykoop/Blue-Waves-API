namespace Esentis.BlueWaves.Persistence.Identity
{
	using System;

	using Esentis.BlueWaves.Persistence.Helpers;

	using Kritikos.Configuration.Persistence.Abstractions;

	using Microsoft.AspNetCore.Identity;

	public class BlueWavesUser : IdentityUser<Guid>, IEntity<Guid>, ITimestamped, IDeletable
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		#region Implementation of IDeletable

		/// <inheritdoc />
		public bool IsDeleted { get; set; }
		#endregion
	}
}
